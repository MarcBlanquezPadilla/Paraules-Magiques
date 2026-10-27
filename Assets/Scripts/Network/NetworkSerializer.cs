using System.IO;
using System.Text;

public static class NetworkSerializer
{
    #region CursorDTO
    public static byte[] Serialize(CursorDTO dto)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(dto.x);
            writer.Write(dto.y);
            return stream.ToArray();
        }
    }

    public static CursorDTO DeserializeCursorDTO(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            CursorDTO dto = new CursorDTO();
            dto.x = reader.ReadSingle();
            dto.y = reader.ReadSingle();
            return dto;
        }
    }
    #endregion

    #region LetterMoveDTO
    public static byte[] Serialize(LetterMoveDTO dto)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(dto.sourceIndex);
            writer.Write(dto.targetIndex);
            writer.Write(dto.isFromMixed);
            return stream.ToArray();
        }
    }

    public static LetterMoveDTO DeserializeLetterMoveDTO(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            LetterMoveDTO dto = new LetterMoveDTO();
            dto.sourceIndex = reader.ReadInt32();
            dto.targetIndex = reader.ReadInt32();
            dto.isFromMixed = reader.ReadBoolean();
            return dto;
        }
    }
    #endregion

    #region LetterDragDTO
    public static byte[] Serialize(LetterDragDTO dto)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(dto.letterIndex);
            writer.Write(dto.isFromMixed);
            writer.Write(dto.x);
            writer.Write(dto.y);
            return stream.ToArray();
        }
    }

    public static LetterDragDTO DeserializeLetterDragDTO(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            LetterDragDTO dto = new LetterDragDTO();
            dto.letterIndex = reader.ReadInt32();
            dto.isFromMixed = reader.ReadBoolean();
            dto.x = reader.ReadSingle();
            dto.y = reader.ReadSingle();
            return dto;
        }
    }
    #endregion

    #region WordStateDTO
    public static byte[] Serialize(WordStateDTO dto)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(dto.originalWord ?? "");
            writer.Write(dto.currentWord ?? "");
            writer.Write(dto.mixedWord ?? "");
            writer.Write(dto.startCurrentWord ?? "");
            writer.Write(dto.startMixedWord ?? "");
            return stream.ToArray();
        }
    }

    public static WordStateDTO DeserializeWordStateDTO(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            WordStateDTO dto = new WordStateDTO();
            dto.originalWord = reader.ReadString();
            dto.currentWord = reader.ReadString();
            dto.mixedWord = reader.ReadString();
            dto.startCurrentWord = reader.ReadString();
            dto.startMixedWord = reader.ReadString();
            return dto;
        }
    }
    #endregion
}