using System;
using System.IO;

[Serializable]
public struct CursorDTO
{
    public float x;
    public float y;
}

[Serializable]
public struct WordStateDTO
{
    public string originalWord;
    public string currentWord;
    public string mixedWord;
    public string startCurrentWord;
    public string startMixedWord;
}

[Serializable]
public struct LetterDragDTO
{
    public int letterIndex;
    public bool isFromMixed;
    public float x;
    public float y;
}

[Serializable]
public struct LetterMoveDTO
{
    public int sourceIndex;
    public int targetIndex;
    public bool isFromMixed;
}

public enum MsgType : byte
{
    Text = 1,
    Cursor = 2,
    WordState = 3,
    LetterMove = 4,
    LetterDrag = 5,
    LetterDragEnd = 6,
    LetterDragCancel = 7,
}

