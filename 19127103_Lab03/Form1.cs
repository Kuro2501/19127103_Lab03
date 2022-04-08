using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _19127103_Lab03
{
    public partial class Form1 : Form
    {
        Color userColor;
        Point pStart, pEnd;
        private string DrawCase = "Line segment";
        bool endSignal = false;
        List<Point> lPoint = new List<Point>();
        shapeList lShape = new shapeList();

        public class AffineTransform
        {
            List<List<double>> TransformMatrix;
            public AffineTransform()
            {
                TransformMatrix = new List<List<double>> {new List<double> {1, 0, 0},
                                                      new List<double> {0, 1, 0},
                                                      new List<double> {0, 0, 1} };
            }
            public void MultiplyMatrix(List<List<double>> Matrix)
            {
                List<List<double>> ResultMatrix = new List<List<double>> {new List<double> {0, 0, 0},
                                                                     new List<double> {0, 0, 0},
                                                                     new List<double> {0, 0, 0} };

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            ResultMatrix[i][j] += Matrix[i][k] * TransformMatrix[k][j];
                        }
                    }
                }
                TransformMatrix = ResultMatrix;
            }

            public void Translate(double dx, double dy)
            {
                List<List<double>> translateMatrix = new List<List<double>> {new List<double> {1, 0, dx },
                                                                             new List<double> {0, 1, dy },
                                                                             new List<double> {0, 0, 1 } };
                MultiplyMatrix(translateMatrix);
            }

            public void Scale(double sx, double sy)
            {
                List<List<double>> scaleMatrix = new List<List<double>> {new List<double> {sx, 0, 0 },
                                                                         new List<double> {0, sy, 0 },
                                                                         new List<double> {0, 0, 1 } };
                MultiplyMatrix(scaleMatrix);
            }

            public void Rotate(double theta)
            {
                double cosTheta = Math.Cos(theta), sinTheta = Math.Sin(theta);
                List<List<double>> rotateMatrix = new List<List<double>> { new List<double> {cosTheta, -sinTheta, 0 },
                                                                           new List<double> {sinTheta, cosTheta, 0 },
                                                                           new List<double> {0, 0, 1 } };
                MultiplyMatrix(rotateMatrix);
            }

            public Point Transform(Point p)
            {
                List<double> thisLocation = new List<double> { p.X, p.Y, 1.0 };
                List<double> newLocation = new List<double> { 0, 0, 0 };
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        newLocation[i] += TransformMatrix[i][j] * thisLocation[j];
                return new Point((int)(Math.Round(newLocation[0])), (int)(Math.Round(newLocation[1])));
            }
        }

        public Form1()
        {
            InitializeComponent();
            userColor = Color.White;
        }

        private void openGLControl1_Load(object sender, EventArgs e)
        {

        }

        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            SharpGL.OpenGL gl = openGLControl1.OpenGL;
            gl.ClearColor(0, 0, 0, 0);
            gl.MatrixMode(SharpGL.OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
        }

        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            SharpGL.OpenGL gl = openGLControl1.OpenGL;
            gl.ClearColor(0, 0, 0, 0);
            gl.MatrixMode(SharpGL.OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, openGLControl1.Width, openGLControl1.Height);
            gl.Ortho2D(0, openGLControl1.Width, 0, openGLControl1.Height);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pStart.X = e.X;
                pStart.Y = openGLControl1.Height - e.Y;
                pEnd = pStart;
            }
            if (endSignal == true)
            {
                lPoint.Clear();
                endSignal = false;
            }
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pEnd.X = e.X;
                pEnd.Y = openGLControl1.Height - e.Y;
                if (DrawCase == "Polygon")
                {
                    lPoint.Add(pEnd);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                endSignal = true;
            }
        }

        private void bt_Palette_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                userColor = colorDialog1.Color;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCase = comboBox1.Text;
            pStart.X = 0;
            pEnd.X = 0;
            pStart.Y = 0;
            pEnd.Y = 0;
        }

        private void plotLineLow(int x0, int y0, int x1, int y1, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int p = 2 * dy - dx;
            int y = y0;
            for (int x = x0; x < x1; x++)
            {
                gl.Vertex(x, y);
                vertexList.Add(new Point(x, y));
                if (p > 0)
                {
                    y = y + yi;
                    p = p + (2 * (dy - dx));
                }
                else
                {
                    p = p + 2 * dy;
                }
            }
        }
        private void plotLineHigh(int x0, int y0, int x1, int y1, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            int p = 2 * dx - dy;
            int x = x0;
            for (int y = y0; y < y1; y++)
            {
                gl.Vertex(x, y);
                vertexList.Add(new Point(x, y));
                if (p > 0)
                {
                    x = x + xi;
                    p = p + (2 * (dx - dy));
                }
                else
                {
                    p = p + 2 * dx;
                }
            }
        }
        private void plotLine(int x0, int y0, int x1, int y1, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            {
                if (x0 > x1)
                    plotLineLow(x1, y1, x0, y0, gl, vertexList);
                else plotLineLow(x0, y0, x1, y1, gl, vertexList);
            }
            else
            {
                if (y0 > y1)
                    plotLineHigh(x1, y1, x0, y0, gl, vertexList);
                else plotLineHigh(x0, y0, x1, y1, gl, vertexList);
            }
        }
        private void plotCircle(int x0, int y0, double r, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int p = (int)(5 / 4 - r);
            int x = 0;
            int y = (int)r;

            while (x <= y)
            {
                gl.Vertex(x0 + x, y0 + y);
                gl.Vertex(x0 - x, y0 + y);
                gl.Vertex(x0 + x, y0 - y);
                gl.Vertex(x0 - x, y0 - y);
                gl.Vertex(x0 + y, y0 + x);
                gl.Vertex(x0 - y, y0 + x);
                gl.Vertex(x0 + y, y0 - x);
                gl.Vertex(x0 - y, y0 - x);

                vertexList.Add(new Point(x0 + x, y0 + y));
                vertexList.Add(new Point(x0 - x, y0 + y));
                vertexList.Add(new Point(x0 + x, y0 - y));
                vertexList.Add(new Point(x0 - x, y0 - y));
                vertexList.Add(new Point(x0 + y, y0 + x));
                vertexList.Add(new Point(x0 - y, y0 + x));
                vertexList.Add(new Point(x0 + y, y0 - x));
                vertexList.Add(new Point(x0 - y, y0 - x));

                if (p < 0)
                {
                    p += 2 * x + 1;
                    x++;
                }
                else
                {
                    p += 2 * (x - y) + 1;
                    x++;
                    y--;
                }
            }
        }
        private void plotRectangle(int beginX, int beginY, int endX, int endY, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            plotLine(beginX, beginY, beginX, endY, gl, vertexList);
            plotLine(beginX, beginY, endX, beginY, gl, vertexList);
            plotLine(beginX, endY, endX, endY, gl, vertexList);
            plotLine(endX, beginY, endX, endY, gl, vertexList);
        }
        private void plotEllipse(int beginX, int beginY, int endX, int endY, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            double rx = Math.Abs((endX - beginX) / 2);
            double ry = Math.Abs((endY - beginY) / 2);
            int x0 = (int)((beginX + endX) / 2);
            int y0 = (int)((beginY + endY) / 2);

            int x, y;
            double dx, dy, p1, p2;
            x = 0;
            y = (int)ry;
            p1 = ry * ry - rx * rx * ry + rx * rx / 4;
            dx = 2 * ry * ry * x;
            dy = 2 * rx * rx * y;
            while (dx < dy)
            {
                gl.Vertex(x0 + x, y0 + y);
                gl.Vertex(x0 - x, y0 + y);
                gl.Vertex(x0 + x, y0 - y);
                gl.Vertex(x0 - x, y0 - y);
                vertexList.Add(new Point(x0 + x, y0 + y));
                vertexList.Add(new Point(x0 - x, y0 + y));
                vertexList.Add(new Point(x0 + x, y0 - y));
                vertexList.Add(new Point(x0 - x, y0 - y));

                if (p1 < 0)
                {
                    x++;
                    dx = dx + 2 * ry * ry;
                    p1 = p1 + dx + ry * ry;
                }
                else
                {
                    x++;
                    y--;
                    dx = dx + 2 * ry * ry;
                    dy = dy - 2 * rx * rx;
                    p1 = p1 + dx - dy + ry * ry;
                }
            }
            p2 = ry * ry * (x + 0.5) * (x + 0.5) + rx * rx * (y - 1) * (y - 1) - rx * rx * ry * ry;
            while (y >= 0)
            {
                if (x != 0)
                {
                    gl.Vertex(x0 + x, y0 + y);
                    gl.Vertex(x0 - x, y0 + y);
                    gl.Vertex(x0 + x, y0 - y);
                    gl.Vertex(x0 - x, y0 - y);
                    vertexList.Add(new Point(x0 + x, y0 + y));
                    vertexList.Add(new Point(x0 - x, y0 + y));
                    vertexList.Add(new Point(x0 + x, y0 - y));
                    vertexList.Add(new Point(x0 - x, y0 - y));

                }
                if (p2 > 0)
                {
                    y--;
                    dy = dy - 2 * rx * rx;
                    p2 = p2 - dy + rx * rx;
                }
                else
                {
                    y--;
                    x++;
                    dx = dx + 2 * ry * ry;
                    dy = dy - 2 * rx * rx;
                    p2 = p2 + dx - dy + rx * rx;
                }
            }
        }
        private void plotTriangle(int beginX, int beginY, int endX, int endY, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int centerX = beginX;
            int centerY = beginY;
            double radius = Math.Sqrt(Math.Pow((endX - beginX), 2) + Math.Pow((endY - beginY), 2));
            double Xbot = radius * Math.Cos((Math.PI / 180) * 30);
            double Ybot = radius * Math.Sin((Math.PI / 180) * 30);
            plotLine(centerX, (int)(centerY + radius), (int)(centerX + Xbot), (int)(centerY - Ybot), gl, vertexList);
            plotLine((int)(centerX + Xbot), (int)(centerY - Ybot), (int)(centerX - Xbot), (int)(centerY - Ybot), gl, vertexList);
            plotLine((int)(centerX - Xbot), (int)(centerY - Ybot), centerX, (int)(centerY + radius), gl, vertexList);
        }
        private void plotPentagon(int beginX, int beginY, int endX, int endY, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int centerX = beginX;
            int centerY = beginY;
            double radius = Math.Sqrt(Math.Pow((endX - beginX), 2) + Math.Pow((endY - beginY), 2));
            double Xtop = radius * Math.Cos((Math.PI / 180) * 18);
            double Ytop = radius * Math.Sin((Math.PI / 180) * 18);
            double Xbot = radius * Math.Sin((Math.PI / 180) * 36);
            double Ybot = radius * Math.Cos((Math.PI / 180) * 36);
            plotLine(centerX, (int)(centerY + radius), (int)(centerX + Xtop), (int)(centerY + Ytop), gl, vertexList);
            plotLine((int)(centerX + Xtop), (int)(centerY + Ytop), (int)(centerX + Xbot), (int)(centerY - Ybot), gl, vertexList);
            plotLine((int)(centerX + Xbot), (int)(centerY - Ybot), (int)(centerX - Xbot), (int)(centerY - Ybot), gl, vertexList);
            plotLine((int)(centerX - Xbot), (int)(centerY - Ybot), (int)(centerX - Xtop), (int)(centerY + Ytop), gl, vertexList);
            plotLine((int)(centerX - Xtop), (int)(centerY + Ytop), centerX, (int)(centerY + radius), gl, vertexList);
        }
        public void plotHexagon(int beginX, int beginY, int endX, int endY, SharpGL.OpenGL gl, List<Point> vertexList)
        {
            int centerX = beginX;
            int centerY = beginY;
            double radius = Math.Sqrt(Math.Pow((endX - beginX), 2) + Math.Pow((endY - beginY), 2));
            double X = radius * Math.Sin((Math.PI / 180) * 60);
            double Y = radius * Math.Cos((Math.PI / 180) * 60);
            plotLine(centerX, (int)(centerY + radius), (int)(centerX + X), (int)(centerY + Y), gl, vertexList);
            plotLine((int)(centerX + X), (int)(centerY + Y), (int)(centerX + X), (int)(centerY - Y), gl, vertexList);
            plotLine((int)(centerX + X), (int)(centerY - Y), centerX, (int)(centerY - radius), gl, vertexList);
            plotLine(centerX, (int)(centerY - radius), (int)(centerX - X), (int)(centerY - Y), gl, vertexList);
            plotLine((int)(centerX - X), (int)(centerY - Y), (int)(centerX - X), (int)(centerY + Y), gl, vertexList);
            plotLine((int)(centerX - X), (int)(centerY + Y), centerX, (int)(centerY + radius), gl, vertexList);
        }
        private void plotPolygon(SharpGL.OpenGL gl)
        {
            gl.Vertex(pEnd.X, pEnd.Y);
            List<Point> pointList = new List<Point>();
            for (int i = 1; i < lPoint.Count(); i++)
            {
                plotLine(lPoint[i - 1].X, lPoint[i - 1].Y, lPoint[i].X, lPoint[i].Y, gl, pointList);
            }
            int n = lPoint.Count() - 1;
            if (endSignal == true)
            {
                List<Point> temp = new List<Point>(lPoint);
                List<Point> control = new List<Point>(lPoint);

                if (lPoint.Count > 0)
                    plotLine(lPoint[n].X, lPoint[n].Y, lPoint[0].X, lPoint[0].Y, gl, pointList);

                lShape.addtoList(control, temp, "Polygon", pointList);
            }
        }
        private void restore(SharpGL.OpenGL gl)
        {
            List<List<Point>> drawingPoint = lShape.getDrawingPointList();
            List<List<Point>> controlPoint = lShape.getControlPointList();
            List<List<Point>> vertexPoint = lShape.getVertexList();
            List<string> optionName = lShape.getOptionNameList();
            int n = drawingPoint.Count;
            for (int i = 1; i < n; i++)
            {
                if (optionName[i] == "Line segment" && i > 1)
                {
                    plotLine(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                }
                if (optionName[i] == "Circle" && i > 1)
                {
                    double radius = Math.Sqrt(Math.Pow((drawingPoint[i][1].X - drawingPoint[i][0].X), 2) + Math.Pow((drawingPoint[i][1].Y - drawingPoint[i][0].Y), 2));
                    plotCircle(drawingPoint[i][0].X, drawingPoint[i][0].Y, radius, gl, new List<Point>());
                }
                if (optionName[i] == "Rectangle" && i > 1)
                {
                    //plotRectangle(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][4].X, controlPoint[i][4].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][2].X, controlPoint[i][2].Y, controlPoint[i][4].X, controlPoint[i][4].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][2].X, controlPoint[i][2].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());

                }
                if (optionName[i] == "Ellipse" && i > 1)
                {
                    //plotEllipse(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                    for (int j = 0; j < vertexPoint[i].Count; j++)
                    {
                        gl.Vertex(vertexPoint[i][j].X, vertexPoint[i][j].Y);
                    }
                }
                if (optionName[i] == "Equilateral triangle" && i > 1)
                {
                    //plotTriangle(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][2].X, controlPoint[i][2].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][2].X, controlPoint[i][2].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());
                }
                if (optionName[i] == "Equilateral pentagon" && i > 1)
                {
                    //plotPentagon(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][2].X, controlPoint[i][2].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][2].X, controlPoint[i][2].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][3].X, controlPoint[i][3].Y, controlPoint[i][4].X, controlPoint[i][4].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][4].X, controlPoint[i][4].Y, controlPoint[i][5].X, controlPoint[i][5].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][5].X, controlPoint[i][5].Y, gl, new List<Point>());
                }
                if (optionName[i] == "Equilateral hexagon" && i > 1)
                {
                    //plotHexagon(drawingPoint[i][0].X, drawingPoint[i][0].Y, drawingPoint[i][1].X, drawingPoint[i][1].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][2].X, controlPoint[i][2].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][2].X, controlPoint[i][2].Y, controlPoint[i][3].X, controlPoint[i][3].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][3].X, controlPoint[i][3].Y, controlPoint[i][4].X, controlPoint[i][4].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][4].X, controlPoint[i][4].Y, controlPoint[i][5].X, controlPoint[i][5].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][5].X, controlPoint[i][5].Y, controlPoint[i][6].X, controlPoint[i][6].Y, gl, new List<Point>());
                    plotLine(controlPoint[i][1].X, controlPoint[i][1].Y, controlPoint[i][6].X, controlPoint[i][6].Y, gl, new List<Point>());
                }
                if (optionName[i] == "Polygon" && i > 1)
                {
                    for (int j = 1; j < drawingPoint[i].Count; j++)
                    {
                        plotLine(drawingPoint[i][j - 1].X, drawingPoint[i][j - 1].Y, drawingPoint[i][j].X, drawingPoint[i][j].Y, gl, new List<Point>());
                    }

                    int lastIndex = drawingPoint[i].Count() - 1;
                    if (drawingPoint[i].Count() > 0)
                        plotLine(drawingPoint[i][lastIndex].X, drawingPoint[i][lastIndex].Y, drawingPoint[i][0].X, drawingPoint[i][0].Y, gl, new List<Point>());
                }
            }
        }

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            SharpGL.OpenGL gl = openGLControl1.OpenGL;
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Color(userColor.R / 255.0, userColor.G / 255.0, userColor.B / 255.0, 0);
            gl.Begin(SharpGL.OpenGL.GL_POINTS);
            if (DrawCase == "Line segment")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                List<Point> control = new List<Point>();
                control.Add(pStart);
                control.Add(pEnd);

                List<Point> pointList = new List<Point>();

                plotLine(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Circle")
            {
                int centerX = pStart.X;
                int centerY = pStart.Y;
                double radius = Math.Sqrt(Math.Pow((pEnd.X - pStart.X), 2) + Math.Pow((pEnd.Y - pStart.Y), 2));
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                List<Point> control = new List<Point>();
                control.Add(pStart);
                control.Add(new Point((int)(centerX - radius), centerY));
                control.Add(new Point((int)(centerX + radius), centerY));
                control.Add(new Point(centerX, (int)(centerY - radius)));
                control.Add(new Point(centerX, (int)(centerY + radius)));

                List<Point> pointList = new List<Point>();

                plotCircle(centerX, centerY, radius, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);

                restore(gl);
            }
            if (DrawCase == "Rectangle")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                List<Point> control = new List<Point>();
                control.Add(new Point(((pEnd.X + pStart.X) / 2), ((pEnd.Y + pStart.Y) / 2)));
                control.Add(pStart);
                control.Add(pEnd);
                control.Add(new Point(pStart.X, pEnd.Y));
                control.Add(new Point(pEnd.X, pStart.Y));

                control.Add(new Point(pStart.X + (pEnd.X - pStart.X) / 2, pEnd.Y));
                control.Add(new Point(pEnd.X - (pEnd.X - pStart.X) / 2, pStart.Y));
                control.Add(new Point(pStart.X, pEnd.Y - (pEnd.Y - pStart.Y) / 2));
                control.Add(new Point(pEnd.X, pStart.Y + (pEnd.Y - pStart.Y) / 2));

                List<Point> pointList = new List<Point>();
                pointList.Add(pStart);

                plotRectangle(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Ellipse")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                List<Point> control = new List<Point>();
                double rx = Math.Abs((pEnd.X - pStart.X) / 2);
                double ry = Math.Abs((pEnd.Y - pStart.Y) / 2);
                int x0 = (int)((pStart.X + pEnd.X) / 2);
                int y0 = (int)((pStart.Y + pEnd.Y) / 2);
                control.Add(new Point(x0, y0));
                control.Add(new Point((int)(x0 + rx), y0));
                control.Add(new Point((int)(x0 - rx), y0));
                control.Add(new Point(x0, (int)(y0 + ry)));
                control.Add(new Point(x0, (int)(y0 - ry)));
                control.Add(pStart);
                control.Add(pEnd);
                control.Add(new Point(pStart.X, pEnd.Y));
                control.Add(new Point(pEnd.X, pStart.Y));

                List<Point> pointList = new List<Point>();
                pointList.Add(pStart);

                plotEllipse(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Equilateral triangle")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                int centerX = pStart.X;
                int centerY = pStart.Y;
                double radius = Math.Sqrt(Math.Pow((pEnd.X - centerX), 2) + Math.Pow((pEnd.Y - centerY), 2));
                double Xbot = radius * Math.Cos((Math.PI / 180) * 30);
                double Ybot = radius * Math.Sin((Math.PI / 180) * 30);
                List<Point> control = new List<Point>();
                control.Add(pStart);
                control.Add(new Point(centerX, (int)(centerY + radius)));
                control.Add(new Point((int)(centerX + Xbot), (int)(centerY - Ybot)));
                control.Add(new Point((int)(centerX - Xbot), (int)(centerY - Ybot)));

                List<Point> pointList = new List<Point>();

                plotTriangle(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Equilateral pentagon")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                int centerX = pStart.X;
                int centerY = pStart.Y;
                double radius = Math.Sqrt(Math.Pow((pEnd.X - centerX), 2) + Math.Pow((pEnd.Y - centerY), 2));
                double Xtop = radius * Math.Cos((Math.PI / 180) * 18);
                double Ytop = radius * Math.Sin((Math.PI / 180) * 18);
                double Xbot = radius * Math.Sin((Math.PI / 180) * 36);
                double Ybot = radius * Math.Cos((Math.PI / 180) * 36);
                List<Point> control = new List<Point>();
                control.Add(pStart);
                control.Add(new Point(centerX, (int)(centerY + radius)));
                control.Add(new Point((int)(centerX + Xtop), (int)(centerY + Ytop)));
                control.Add(new Point((int)(centerX + Xbot), (int)(centerY - Ybot)));
                control.Add(new Point((int)(centerX - Xbot), (int)(centerY - Ybot)));
                control.Add(new Point((int)(centerX - Xtop), (int)(centerY + Ytop)));

                List<Point> pointList = new List<Point>();

                plotPentagon(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Equilateral hexagon")
            {
                List<Point> temp = new List<Point>();
                temp.Add(pStart);
                temp.Add(pEnd);

                int centerX = pStart.X;
                int centerY = pStart.Y;
                double radius = Math.Sqrt(Math.Pow((pEnd.X - centerX), 2) + Math.Pow((pEnd.Y - centerY), 2));
                double X = radius * Math.Sin((Math.PI / 180) * 60);
                double Y = radius * Math.Cos((Math.PI / 180) * 60);
                List<Point> control = new List<Point>();
                control.Add(pStart);
                control.Add(new Point(centerX, (int)(centerY + radius)));
                control.Add(new Point((int)(centerX + X), (int)(centerY + Y)));
                control.Add(new Point((int)(centerX + X), (int)(centerY - Y)));
                control.Add(new Point(centerX, (int)(centerY - radius)));
                control.Add(new Point((int)(centerX - X), (int)(centerY - Y)));
                control.Add(new Point((int)(centerX - X), (int)(centerY + Y)));


                List<Point> pointList = new List<Point>();

                plotHexagon(pStart.X, pStart.Y, pEnd.X, pEnd.Y, gl, pointList);
                lShape.addtoList(control, temp, DrawCase, pointList);
                restore(gl);
            }
            if (DrawCase == "Polygon")
            {
                plotPolygon(gl);
                restore(gl);
            }
            if (DrawCase == "Choose object")
            {
                restore(gl);

                int pos = lShape.findObject(pStart);

                if (pos != -1)
                {
                    List<Point> ctrlPoint = lShape.getObjCtrlPointList(pos);
                    
                    int n = ctrlPoint.Count();
                    for (int i = 0; i < n; i++)
                    {
                        plotRectangle(ctrlPoint[i].X - 2, ctrlPoint[i].Y - 2, ctrlPoint[i].X + 2, ctrlPoint[i].Y + 2, gl, new List<Point>());
                    }

                    // Các phép biến đổi Affine
                    List<Point> vertex = lShape.getObjVertexList(pos);
                    List<Point> drawing = lShape.getObjDrawingList(pos);
                    String option = lShape.getObjDrawingOption(pos);

                    AffineTransform af = new AffineTransform();

                    Point center = ctrlPoint[0];

                    // Tính scale
                    double p2 = Math.Sqrt(Math.Pow((pStart.X - center.X), 2) + Math.Pow((pStart.Y - center.Y), 2));
                    double p1 = Math.Sqrt(Math.Pow((pEnd.X - center.X), 2) + Math.Pow((pEnd.Y - center.Y), 2));
                    double scale = p1 / p2;
                    // Tính góc
                    Point vector1 = new Point(pStart.X - center.X, pStart.Y - center.Y);
                    Point vector2 = new Point(pEnd.X - center.X, pEnd.Y - center.Y);

                    double length1 = Math.Sqrt(Math.Pow((pStart.X - center.X), 2) + Math.Pow((pStart.Y - center.Y), 2));
                    double length2 = Math.Sqrt(Math.Pow((pEnd.X - center.X), 2) + Math.Pow((pEnd.Y - center.Y), 2));

                    double cosTheta = ((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) / (length1 * length2);
                    double theta = Math.Acos(cosTheta);

                    if ((vector1.X * vector2.Y) - (vector1.Y * vector2.X) < 0) { theta = -theta; }

                    if (lShape.isControlPoint(pos, pStart))
                    {
                        af.Translate(0 - center.X, 0 - center.Y);
                        af.Scale(scale, scale);
                        af.Rotate(theta);
                        af.Translate(center.X, center.Y);
                    }
                    else
                    {
                        af.Translate((pEnd.X - pStart.X), (pEnd.Y - pStart.Y));
                    }

                    for (int i = 0; i < vertex.Count; i++)
                    {
                        vertex[i] = af.Transform(vertex[i]);
                        if (i < drawing.Count)
                        {
                            drawing[i] = af.Transform(drawing[i]);
                        }
                        if (i < ctrlPoint.Count)
                        {
                            ctrlPoint[i] = af.Transform(ctrlPoint[i]);
                        }
                    }
                    lShape.update(pos, ctrlPoint, drawing, vertex);
                }

            }
            gl.End();
            gl.Flush();
        }

    }
}
