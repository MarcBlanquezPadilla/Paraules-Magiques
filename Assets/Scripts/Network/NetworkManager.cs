using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerName();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Network Configuration")]
    public bool isMultiplayer = false;
    public bool isServer = false;
    public string serverIP = "127.0.0.1";
    public int port = 9050;

    [Header("Connection Status")]
    public bool isConnected = false;

    [Header("Player Info")]
    public string playerName = "Jugador";
    public string remotePlayerName = "Esperant...";

    [Header("Remote Player State")]
    public Vector2 remoteCursorPosition;
    public bool hasNewRemoteCursor = false;

    private Socket m_socket;
    private EndPoint m_remoteEP;
    private Thread m_listenThread;
    private volatile bool m_isRunning = false;

    private struct Packet
    {
        public byte[] data;
        public EndPoint from;
    }

    private readonly ConcurrentQueue<Packet> m_inbox = new ConcurrentQueue<Packet>();
    private readonly HashSet<EndPoint> m_clients = new HashSet<EndPoint>();

    private void LoadPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
        }
        else
        {
            int randomNumber = UnityEngine.Random.Range(1, 10000);
            playerName = "Jugador" + randomNumber.ToString("D4");
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }
    }

    public void ChangePlayerName(string name)
    {
        playerName = name;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }

    public void StartHost()
    {
        isMultiplayer = true;
        isServer = true;
        StartNetwork();
    }

    public void StartClient(string ip)
    {
        isMultiplayer = true;
        isServer = false;
        serverIP = ip;
        StartNetwork();
    }

    public void StopNetwork()
    {
        m_isRunning = false;
        if (m_socket != null)
        {
            try { m_socket.Close(); } catch { }
            m_socket = null;
        }

        if (m_listenThread != null && m_listenThread.IsAlive)
        {
            m_listenThread.Join(500);
            m_listenThread = null;
        }

        lock (m_clients)
        {
            m_clients.Clear();
        }

        remotePlayerName = "Esperant...";
        isConnected = false;
    }

    private void StartNetwork()
    {
        StopNetwork();

        try
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            if (isServer)
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Any, port));
            }
            else
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                m_remoteEP = new IPEndPoint(IPAddress.Parse(serverIP), port);
            }

            m_isRunning = true;
            m_listenThread = new Thread(ListenLoop) { IsBackground = true };
            m_listenThread.Start();

            if (!isServer)
            {
                SendText("JOIN:" + playerName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetworkManager] Error en el socket UDP: {e.Message}");
        }
    }

    public bool AreBothPlayersConnected
    {
        get
        {
            if (!isMultiplayer) return false;

            if (isServer)
            {
                lock (m_clients)
                {
                    return m_clients.Count > 0;
                }
            }
            else
            {
                return isConnected;
            }
        }
    }

    private void ListenLoop()
    {
        byte[] buffer = new byte[2048];

        while (m_isRunning)
        {
            try
            {
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                int received = m_socket.ReceiveFrom(buffer, ref remote);

                if (received > 0)
                {
                    byte[] payload = new byte[received];
                    Array.Copy(buffer, payload, received);
                    m_inbox.Enqueue(new Packet { data = payload, from = remote });
                }
            }
            catch
            {
                if (!m_isRunning) break;
            }
        }
    }

    private void Update()
    {
        if (!isMultiplayer) return;

        Packet packet;
        while (m_inbox.TryDequeue(out packet))
        {
            ProcessPacket(packet.data, packet.from);
        }
    }

    private void ProcessPacket(byte[] data, EndPoint from)
    {
        if (data == null || data.Length < 1) return;

        isConnected = true;

        if (isServer)
        {
            lock (m_clients)
            {
                m_clients.Add(from);
                m_remoteEP = from;
            }
        }

        MsgType type = (MsgType)data[0];
        byte[] body = new byte[data.Length - 1];
        Array.Copy(data, 1, body, 0, body.Length);

        switch (type)
        {
            case MsgType.Cursor:
                CursorDTO cursor = NetworkSerializer.DeserializeCursorDTO(body);
                remoteCursorPosition = new Vector2(cursor.x, cursor.y);
                hasNewRemoteCursor = true;
                break;

            case MsgType.WordState:
                WordStateDTO wordState = NetworkSerializer.DeserializeWordStateDTO(body);
                WordsManager wordsMgr = FindFirstObjectByType<WordsManager>();
                if (wordsMgr != null)
                {
                    wordsMgr.UpdateWordState(wordState);
                }
                break;

            case MsgType.LetterDrag:
                LetterDragDTO dragPos = NetworkSerializer.DeserializeLetterDragDTO(body);
                WordsManager wm = FindFirstObjectByType<WordsManager>();
                if (wm != null)
                {
                    wm.HandleRemoteDrag(dragPos);
                }
                break;

            case MsgType.LetterDragEnd:
                LetterDragDTO drag = NetworkSerializer.DeserializeLetterDragDTO(body);
                WordsManager wordm = FindFirstObjectByType<WordsManager>();
                if (wordm != null)
                {
                    wordm.HandleRemoteDragEnd(drag);
                }
                break;
            
            case MsgType.LetterDragCancel:
                LetterDragDTO dragC = NetworkSerializer.DeserializeLetterDragDTO(body);
                WordsManager wordmana = FindFirstObjectByType<WordsManager>();
                if (wordmana != null)
                {
                    wordmana.HandleDragCancel(dragC);
                }
                break;

            case MsgType.LetterMove:
                LetterMoveDTO move = NetworkSerializer.DeserializeLetterMoveDTO(body);
                if (isServer)
                {
                    WordsManager serverWordsMgr = FindFirstObjectByType<WordsManager>();
                    if (serverWordsMgr != null)
                    {
                        serverWordsMgr.MoveLetter(move);
                    }
                }
                break;

            case MsgType.Text:
                
                string text = Encoding.UTF8.GetString(body);

                if (text.StartsWith("JOIN"))
                {
                    if (isServer)
                    {
                        lock (m_clients)
                        {
                            if (!m_clients.Contains(from))
                            {
                                m_clients.Add(from);
                                Debug.Log($"[HOST] ¡Nuevo cliente registrado desde {from}! Total: {m_clients.Count}");
                            }
                            m_remoteEP = from;
                        }

                        string[] parts = text.Split(':');
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                        {
                            remotePlayerName = parts[1];
                        }
                        
                        SendText("WELCOME:" + playerName);

                        WordsManager serverWordsMgr = FindFirstObjectByType<WordsManager>();
                        if (serverWordsMgr != null)
                        {
                            SendWordState(serverWordsMgr.GetOriginalWord(), serverWordsMgr.GetCurrentWord(), serverWordsMgr.GetMixedCurrentWord(), serverWordsMgr.GetStartCurrentWord(), serverWordsMgr.GetStartMixedWord());
                        }
                    }
                }
                else if (text.StartsWith("WELCOME"))
                {
                    string[] parts = text.Split(':');
                    if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                    {
                        remotePlayerName = parts[1];
                    }

                    isConnected = true;

                    SendText("NAME_SYNC:" + playerName);
                }
                else if (text.StartsWith("NAME_SYNC") && isServer)
                {
                    string[] parts = text.Split(':');
                    if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                    {
                        remotePlayerName = parts[1];
                    }
                }
                else if (text == "START")
                {
                    ScenesManager.Instance.ChangeScene("SampleScene");
                }
                else if (text == "SYNC_BOARD" && isServer)
                {
                    WordsManager serverWordsMgr = FindFirstObjectByType<WordsManager>();
                    if (serverWordsMgr != null)
                    {
                        SendWordState(serverWordsMgr.GetOriginalWord(), serverWordsMgr.GetCurrentWord(), serverWordsMgr.GetMixedCurrentWord(), serverWordsMgr.GetStartCurrentWord(), serverWordsMgr.GetStartMixedWord());
                    }
                }
                else if (text == "SOLVED" && !isServer)
                {
                    WordsManager clientWordsMgr = FindFirstObjectByType<WordsManager>();
                    if (clientWordsMgr != null)
                    {
                        clientWordsMgr.SetSolved(true);
                    }
                }
                else if (text == "WRONG" && !isServer)
                {
                    WordsManager clientWordsMgr = FindFirstObjectByType<WordsManager>();
                    if (clientWordsMgr != null)
                    {
                        clientWordsMgr.SetSolved(false);
                    }
                }
                else if (text == "DISCONNECT")
                {
                    StopNetwork();

                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SampleScene")
                    {
                        ScenesManager.Instance.ChangeScene("MainMenu");
                    }
                }
                break;
        }
    }

    #region Send Methods
    public void SendCursor(float x, float y)
    {
        if (!isMultiplayer || m_socket == null) return;

        CursorDTO dto = new CursorDTO { x = x, y = y };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.Cursor, body);
    }

    public void SendWordState(string originalWord, string currentWord, string mixedWord, string startCurrentWord, string startMixedWord)
    {
        if (!isMultiplayer || m_socket == null) return;

        WordStateDTO dto = new WordStateDTO
        {
            originalWord = originalWord,
            currentWord = currentWord,
            mixedWord = mixedWord,
            startCurrentWord = startCurrentWord,
            startMixedWord = startMixedWord
        };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.WordState, body);
    }

    public void SendLetterMove(int sourceIndex, int targetIndex, bool isFromMixed)
    {
        if (!isMultiplayer || m_socket == null) return;

        LetterMoveDTO dto = new LetterMoveDTO { sourceIndex = sourceIndex, targetIndex = targetIndex, isFromMixed = isFromMixed };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.LetterMove, body);
    }


    public void SendLetterDrag(int index, bool isFromMixed, float x, float y)
    {
        if (!isMultiplayer || m_socket == null) return;
        LetterDragDTO dto = new LetterDragDTO { letterIndex = index, isFromMixed = isFromMixed, x = x, y = y };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.LetterDrag, body);
    }

    public void SendLetterDragEnd(int index, bool isFromMixed, float x, float y)
    {
        if (!isMultiplayer || m_socket == null) return;
        LetterDragDTO dto = new LetterDragDTO { letterIndex = index, isFromMixed = isFromMixed, x = x, y = y };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.LetterDragEnd, body);
    }

    public void SendDragCancel(int index, bool isFromMixed)
    {
        if (!isMultiplayer || m_socket == null) return;
        LetterDragDTO dto = new LetterDragDTO { letterIndex = index, isFromMixed = isFromMixed, x = 0, y = 0 };
        byte[] body = NetworkSerializer.Serialize(dto);
        SendPacket(MsgType.LetterDragCancel, body);
    }

    public void SendText(string text)
    {
        if (!isMultiplayer || m_socket == null) return;

        byte[] body = Encoding.UTF8.GetBytes(text);
        SendPacket(MsgType.Text, body);
    }

    private void SendPacket(MsgType type, byte[] body)
    {
        byte[] message = new byte[1 + body.Length];
        message[0] = (byte)type;
        body.CopyTo(message, 1);

        try
        {
            if (isServer)
            {
                lock (m_clients)
                {
                    foreach (EndPoint clientEP in m_clients)
                    {
                        m_socket.SendTo(message, clientEP);
                    }
                }
            }
            else if (m_remoteEP != null)
            {
                m_socket.SendTo(message, m_remoteEP);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetworkManager] Error al enviar paquete ({type}): {e.Message}");
        }
    }
    #endregion

    private void OnDestroy()
    {
        StopNetwork();
    }

    private void OnApplicationQuit()
    {
        if (isMultiplayer && m_socket != null)
        {
            SendText("DISCONNECT");
            StopNetwork();
        }
    }
}