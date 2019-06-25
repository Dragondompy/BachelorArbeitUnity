﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class ObjMesh
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<List<int>> faces = new List<List<int>>();
        private string comments = "";

        //creates the ObjMesh from the lists of vertices and faces
        public ObjMesh(List<Vector3> vertices, List<List<int>> faces, String comments)
        {
            setVertices(vertices);
            setFaces(faces);
            setComments(comments + "\n");
        }

        //creates the objmesh from an Mesh m
        public ObjMesh(Mesh m)//TODO handle deleted vertices and meshes !!!
        {
            foreach (Vertex vertex in m.getVertices())
            {
                if (vertex.isValid())
                {
                    vertices.Add(vertex.getPosition());
                }
            }
            foreach (Face face in m.getFaces())
            {
                if (face.isValid())
                {
                    List<int> fVertices = new List<int>();
                    foreach (Vertex vertex in face.getVertices())
                    {
                        fVertices.Add(vertex.getHandleNumber());
                    }
                    faces.Add(fVertices);
                }
            }
            comments += "# " + m.getVertexHandleNumber() + " Vertices " + m.getFaceHandleNumber() + " Faces\n";
        }
        //creates the objmesh from an obj file
        public ObjMesh(string fileName)
        {
            int lineTest = 0;
            string[] curLine;
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (sr.Peek() > 0)
                    {
                        lineTest++;
                        line = sr.ReadLine();
                        line = string.Join(" ", line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                        curLine = line.Split(' ');
                        List<int> faceVertices = new List<int>();

                        if (curLine[0].Equals("v"))
                        {
                            vertices.Add(new Vector3(
                                Single.Parse(curLine[1].Replace('.', ',')),
                                Single.Parse(curLine[2].Replace('.', ',')),
                                Single.Parse(curLine[3].Replace('.', ','))
                            ));

                        }
                        else if (curLine[0].Equals("f"))
                        {
                            for (int f = 1; f < curLine.Length; f++)
                            {
                                faceVertices.Add((Int32.Parse(curLine[f])) - 1);
                            }
                            faces.Add(faceVertices);
                        }
                        else if (curLine[0].Equals("#"))
                        {
                            comments += line + "\n";
                        }
                    }
                    sr.Close();
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Obj-File has wrong Format in Line: " + lineTest);
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void writeToFile(String fileName)
        {
            String path = "./Assets/Data/" + fileName + ".obj";
            if (File.Exists(path))
            {
                Console.WriteLine("File already exists. Override File ? y/n");
                String answer = Console.ReadLine();
                if (!answer.Equals("y") && !answer.Equals("yes"))
                {
                    Console.WriteLine("Mesh not saved");
                    return;
                }
                else
                {
                    File.Delete(path);
                }
            }
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(this);
            }
        }

        public string getComments()
        {
            return comments;
        }

        public List<Vector3> getVertices()
        {
            return vertices;
        }

        public List<List<int>> getFaces()
        {
            return faces;
        }

        public void setComments(string comments)
        {
            this.comments = comments;
        }

        public void setVertices(List<Vector3> vertices)
        {
            this.vertices = vertices;
        }

        public void setFaces(List<List<int>> faces)
        {
            this.faces = faces;
        }

        public override string ToString()
        {
            string output = comments;
            foreach (Vector3 v in vertices)
            {
                output += "v " + v.x + " " + v.y + " " + v.z + "\n";
            }
            foreach (List<int> fV in faces)
            {
                output += "f ";
                foreach (int i in fV)
                {
                    output += (i + 1) + " ";
                }
                output += "\n";
            }
            output = output.Replace(',', '.');
            return output;
        }
    }
}
