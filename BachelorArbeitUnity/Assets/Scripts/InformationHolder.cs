﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BachelorArbeitUnity;

public static class InformationHolder {
    public static string pathToMesh;
    public static bool selectVertices;
    public static bool selectEdge;
    public static bool selectFace;
    public static bool showOriginal;
    public static bool showPatches;
    public static bool showNewMesh;
    public static float camSpeed;
    public static float camRotSpeed;
    public static float threshHold;
    public static Controller con;
    public static int[] myMeshToPatchHolder;
    public static List<int> PatchHolderToNewMesh;
}
