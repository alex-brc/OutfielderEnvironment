using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
public static class DataTags
{
    public static readonly char MainSeparator = ':', SecondarySeparator = ';';

    public static class Result
    {
        public static readonly string
            Success = "SUCCESS",
            Failure = "FAILURE";
    }

    public static class EntryTypes
    {
        public static readonly string
            Result = "RESULT",
            Camera = "CAMERA",
            GameObject = "GAMEOBJECT",
            FOVE = "FOVE",
            WiiBoard = "BOARD";
        // There is no acceleration in Unity
    }
    
    public static class FOVE
    {
        public static readonly string
            Nan = null;
            // To be completed
    }

    public static class Board
    {
        public static readonly string
            Nan = null;
        // To be completed
    }


}
