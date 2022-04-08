using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _19127103_Lab03
{
    class shapeList
    {
        private List<List<Point>> ctrlPoint;
        private List<List<Point>> drawingPoint;
        private List<string> optionName;
        private List<List<Point>> vertexPoint;

        public shapeList()
        {
            ctrlPoint = new List<List<Point>>();
            drawingPoint = new List<List<Point>>();
            optionName = new List<string>();
            vertexPoint = new List<List<Point>>();
        }

        public shapeList(List<List<Point>> crtl, List<List<Point>> draw, List<string> option)
        {
            ctrlPoint = crtl;
            drawingPoint = draw;
            optionName = option;
        }

        public List<List<Point>> getControlPointList()
        {
            return ctrlPoint;
        }

        public List<List<Point>> getVertexList()
        {
            return vertexPoint;
        }

        public List<Point> getObjCtrlPointList(int pos)
        {
            return ctrlPoint[pos];
        }

        public String getObjDrawingOption(int pos)
        {
            return optionName[pos];
        }

        public List<List<Point>> getDrawingPointList()
        {
            return drawingPoint;
        }

        public List<string> getOptionNameList()
        {
            return optionName;
        }

        public string getObjOptionName(int pos)
        {
            return optionName[pos];
        }

        public List<Point> getObjVertexList(int pos)
        {
            return vertexPoint[pos];
        }

        public List<Point> getObjDrawingList(int pos)
        {
            return drawingPoint[pos];
        }

        public void addtoList(List<Point> ctrlList, List<Point> drawList, string optName, List<Point> vertexList)
        {
            ctrlPoint.Add(ctrlList);
            drawingPoint.Add(drawList);
            optionName.Add(optName);
            vertexPoint.Add(vertexList);
        }

        public void update(int pos, List<Point> ctrlList, List<Point> drawList, List<Point> vertexList)
        {
            ctrlPoint[pos] = ctrlList;
            drawingPoint[pos] = drawList;
            vertexPoint[pos] = vertexList;
        }

        public void clearScreen()
        {
            ctrlPoint.Clear();
            drawingPoint.Clear();
            optionName.Clear();
        }

        public int findObject(Point pointer)
        {
            int pos = -1;
            double min = 5;
            int n = vertexPoint.Count();
            for (int i = 0; i < n; i++)
            {
                int m = vertexPoint[i].Count();
                for (int j = 0; j < m; j++)
                {
                    if (calculateRange(vertexPoint[i][j], pointer) < min)
                    {
                        min = calculateRange(vertexPoint[i][j], pointer);
                        pos = i;
                    }
                }
            }
            return pos;
        }

        public bool isControlPoint(int pos, Point pointer)
        {
            double min = 7;
            int n = ctrlPoint[pos].Count();
            for (int i = 0; i < n; i++)
            {
                if (calculateRange(ctrlPoint[pos][i], pointer) < min)
                    return true;
            }
            return false;
        }

        public double calculateRange(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }
    }
}
