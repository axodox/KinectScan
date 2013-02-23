using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace KinectScan
{
    public class TextureDoubleBuffer : IDisposable
    {
        public int TextureCount { get; private set; }
        GraphicsDevice Device;
        Texture2D[] Textures;
        public int FrontTextureIndex { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureDoubleBuffer(GraphicsDevice device, int width, int height, SurfaceFormat format, int bufferSize = 2)
        {
            TextureCount = bufferSize;
            Device = device;
            Textures = new Texture2D[TextureCount];
            Width = width;
            Height = height;
            for (int i = 0; i < TextureCount; i++)
            {
                Textures[i] = new Texture2D(device, width, height, false, format);
            }
            TextureInWrite = -1;
            TextureInRead = -1;
            FrontTextureIndex = -1;
        }

        public int TextureInWrite { get; private set; }
        public int TextureInRead { get; private set; }

        //public void SetData(byte[] data)
        //{
        //    if (Textures != null)
        //    {
        //        int id = FrontTextureIndex;
        //        do
        //        {
        //            id = (id + 1) % TextureCount;
        //        }
        //        while (id == TextureInRead || id == FrontTextureIndex);
        //        TextureInWrite = id;
        //        for (int i = 0; i < 16; i++)
        //            if (Device.Textures[i] == Textures[TextureInWrite])
        //            {
        //                Device.Textures[i] = null;
        //            }
        //        Textures[TextureInWrite].SetData<byte>(data);
        //        FrontTextureIndex = TextureInWrite;
        //        TextureInWrite = -1;
        //    }
        //}

        public void SetData<T>(T[] data) where T : struct
        {
            if (Textures != null)
            {
                int id = FrontTextureIndex;
                do
                {
                    id = (id + 1) % TextureCount;
                }
                while (id == TextureInRead || id == FrontTextureIndex);
                TextureInWrite = id;
                for (int i = 0; i < 16; i++)
                    if (Device.Textures[i] == Textures[TextureInWrite])
                    {
                        Device.Textures[i] = null;
                    }
                Textures[TextureInWrite].SetData<T>(data);
                FrontTextureIndex = TextureInWrite;
                TextureInWrite = -1;
            }
        }

        public Texture2D BeginTextureUse(int id)
        {
            if (id == TextureInWrite || TextureInRead != -1 || id==-1)
            {
                return null;
            }
            else
            {
                TextureInRead = id;
                return Textures[id];
            }
        }

        public void EndTextureUse()
        {
            TextureInRead = -1;
        }

        public Texture2D FrontTexture
        {
            get
            {
                if (FrontTextureIndex == -1)
                {
                    return null;
                }
                else
                {
                    return Textures[FrontTextureIndex];
                }
            }
        }

        public void Dispose()
        {
            if (Textures != null)
            {
                for (int i = 0; i < TextureCount; i++)
                {
                    Textures[i].Dispose();
                }
                Textures = null;
                FrontTextureIndex = -1;
            }
        }
    }

    public class XPlane
    {
        private VertexPositionTexture[] Vertices;
        private int[] Indicies;
        private VertexBuffer VB;
        private IndexBuffer IB;
        private GraphicsDevice GD;
        private int TriangleCount;
        public XPlane(int width, int height)
        {
            float xstep = 1f / (width - 1), xtexstep = 1f / (width-1), xstart = -0.5f;
            float ystep = -1f / (height - 1) /** height / width*/, ytexstep = 1f / (height-1), ystart = 0.5f /** height / width*/;

            Vertices = new VertexPositionTexture[width * height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Vertices[j * width + i] = new VertexPositionTexture(
                        new Vector3(xstart + i * xstep, ystart + j * ystep, 0f),
                        new Vector2(i * xtexstep, j * ytexstep));
                }
            }

            int triangleWidth = width - 1, triangleHeight = height - 1;
            Indicies = new int[triangleWidth * triangleHeight * 6];
            int triangleIndex;
            for (int j = 0; j < triangleHeight; j++)
            {
                for (int i = 0; i < triangleWidth; i++)
                {
                    triangleIndex = (j * triangleWidth + i) * 6;
                    Indicies[triangleIndex] = j * width + i;
                    Indicies[triangleIndex + 1] = j * width + i + 1;
                    Indicies[triangleIndex + 2] = (j + 1) * width + i;
                    Indicies[triangleIndex + 3] = j * width + i + 1;
                    Indicies[triangleIndex + 4] = (j + 1) * width + i + 1;
                    Indicies[triangleIndex + 5] = (j + 1) * width + i;
                }
            }

            TriangleCount = triangleWidth * triangleHeight * 2;
        }

        public void SetDevice(GraphicsDevice device)
        {
            GD = device;
            if (VB != null)
            {
                VB.Dispose();
                VB = null;
            }
            if (IB != null)
            {
                IB.Dispose();
                IB = null;
            }
            if (GD != null)
            {
                VB = new VertexBuffer(GD, VertexPositionTexture.VertexDeclaration, Vertices.Length, BufferUsage.None);
                VB.SetData<VertexPositionTexture>(Vertices);
                IB = new IndexBuffer(GD, IndexElementSize.ThirtyTwoBits, Indicies.Length, BufferUsage.None);
                IB.SetData<int>(Indicies);
            }
        }

        public void Draw()
        {
            GD.SetVertexBuffer(VB);
            GD.Indices = IB;
            GD.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VB.VertexCount, 0, TriangleCount);
        }
    }

    class XRectangle
    {
        private VertexPositionColor[] Vertices;
        private VertexBuffer VB;
        private GraphicsDevice GD;
        public XRectangle(float x1, float y1, float x2, float y2, Microsoft.Xna.Framework.Color color)
        {
            Vertices = new VertexPositionColor[5];
            UpdatePosition(x1, y1, x2, y2, color);
        }

        public void UpdatePosition(float x1, float y1, float x2, float y2, Microsoft.Xna.Framework.Color color)
        {
            Vertices[0] = new VertexPositionColor(new Vector3(x1, y1, 0f), color);
            Vertices[1] = new VertexPositionColor(new Vector3(x2, y1, 0f), color);
            Vertices[2] = new VertexPositionColor(new Vector3(x2, y2, 0f), color);
            Vertices[3] = new VertexPositionColor(new Vector3(x1, y2, 0f), color);
            Vertices[4] = new VertexPositionColor(new Vector3(x1, y1, 0f), color);
            if (VB != null)
            {
                VB.SetData<VertexPositionColor>(Vertices);
            }
        }

        public void SetDevice(GraphicsDevice device)
        {
            GD = device;
            VB = new VertexBuffer(GD, VertexPositionColor.VertexDeclaration, 5, BufferUsage.WriteOnly);
            VB.SetData<VertexPositionColor>(Vertices);
        }

        public void Draw()
        {
            GD.SetVertexBuffer(VB);
            GD.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
            GD.SetVertexBuffer(null);
        }
    }
}
