using Oddmatics.Rzxe.Windowing.Graphics;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using Pencil.Gaming;
using Oddmatics.Rzxe.Game;

namespace Oddmatics.Rzxe.Windowing.Implementations.GlfwFx
{
    internal sealed class GLSpriteBatch : ISpriteBatch
    {
        private GLGraphicsController OwnerController { get; set; }

        #region GL Stuff

        private int GlCanvasResolutionUniformId { get; set; }

        private uint GlProgramId { get; set; }

        private int GlUvMapResolutionUniformId { get; set; }

        private List<float> VboDrawContents { get; set; }

        private List<float> VboUvContents { get; set; }

        private int VertexCount { get; set; }

        #endregion

        #region Resource Stuff

        private GLResourceCache ResourceCache { get; set; }

        private GLSpriteAtlas SpriteAtlas { get; set; }

        #endregion


        public GLSpriteBatch(
            GLGraphicsController owner,
            string atlasName,
            int type,
            IList<TextItem> bitmapList,
            GLResourceCache resourceCache
            )
        {
            OwnerController = owner;

            // Set up resource bits
            //
            ResourceCache = resourceCache;
            if (type != 3)
            {
                SpriteAtlas = ResourceCache.GetAtlas(atlasName, type, bitmapList);
            }

            // Set up GL fields
            //
            GlProgramId = ResourceCache.GetShaderProgram("SimpleUVs"); // FIXME: Hard-coded ew!

            GlCanvasResolutionUniformId = GL.GetUniformLocation(
                GlProgramId,
                "CanvasResolution"
                );

            GlUvMapResolutionUniformId = GL.GetUniformLocation(
                GlProgramId,
                "UvMapResolution"
                );

            VertexCount = 0;
            VboDrawContents = new List<float>();
            VboUvContents = new List<float>();
        }

        public Rectanglei GetSpriteUV(string spriteName)
        {
            Dictionary<string, Rectanglei> spriteMap = SpriteAtlas.GetSpriteMap;
            
            return spriteMap[spriteName];
        }

        public void DeleteAtlas(string atlas)
        {
            ResourceCache.DeleteAtalas(atlas);
        }
        public int GetAtlasLength()
        {
            Dictionary<string, Rectanglei> spriteMap = SpriteAtlas.GetSpriteMap;
            return spriteMap.Count();
        }
        public void Draw(string spriteName, System.Drawing.Rectangle rect)
        {
            Rectanglei spriteRect = SpriteAtlas.GetSpriteUV(spriteName);

            VboDrawContents.AddRange(GLUtility.MakeVboData(rect));
            VboUvContents.AddRange(GLUtility.MakeVboData(spriteRect));

            VertexCount += 12;
        }

        
        public void Finish()
        {
            // Create VBO for the batch
            //
            int vboDrawId = GL.GenBuffer();
            int vboUvId = GL.GenBuffer();
            float[] spriteDrawVerts = VboDrawContents.ToArray();
            float[] spriteUvVerts = VboUvContents.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteDrawVerts.Length),
                spriteDrawVerts,
                BufferUsageHint.StreamDraw
                );

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteUvVerts.Length),
                spriteUvVerts,
                BufferUsageHint.StreamDraw
                );

            // Set up shader program
            //
            GL.UseProgram(GlProgramId);

            GL.Uniform2(
                GlCanvasResolutionUniformId,
                (float)OwnerController.TargetResolution.Width,
                (float)OwnerController.TargetResolution.Height
                );

            GL.Uniform2(GlUvMapResolutionUniformId, SpriteAtlas.Size);

            // Bind the atlas
            //
            GL.BindTexture(
                TextureTarget.Texture2D,
                SpriteAtlas.GlTextureId
                );

            // Assign draw attrib
            //
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.VertexAttribPointer(
                0,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );

            // Assign UV attrib
            //
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );


            // Draw now!
            //
            GL.DrawArrays(BeginMode.Triangles, 0, VertexCount);

            // Detach and destroy VBOs
            //
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.DeleteBuffer(vboDrawId);
            GL.DeleteBuffer(vboUvId);
        }
        public void FinishFrame(int frame)
        {
            // Create VBO for the batch
            //
            
            int vboDrawId = GL.GenBuffer();
            int vboUvId = GL.GenBuffer();
            float[] spriteDrawVerts = VboDrawContents.ToArray();
            float[] spriteUvVerts = VboUvContents.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteDrawVerts.Length),
                spriteDrawVerts,
                BufferUsageHint.StreamDraw
                );

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteUvVerts.Length),
                spriteUvVerts,
                BufferUsageHint.StreamDraw
                );

            // Set up shader program
            //
            GL.UseProgram(GlProgramId);

            GL.Uniform2(
                GlCanvasResolutionUniformId,
                (float)OwnerController.TargetResolution.Width,
                (float)OwnerController.TargetResolution.Height
                );

            GL.Uniform2(GlUvMapResolutionUniformId, SpriteAtlas.Size);

            // Bind the atlas
            //
            GL.BindTexture(
                TextureTarget.Texture2D,
                SpriteAtlas.GifMap[frame]
                );

            // Assign draw attrib
            //
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.VertexAttribPointer(
                0,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );

            // Assign UV attrib
            //
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );


            // Draw now!
            //
            GL.DrawArrays(BeginMode.Triangles, 0, VertexCount);

            // Detach and destroy VBOs
            //
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.DeleteBuffer(vboDrawId);
            GL.DeleteBuffer(vboUvId);
        }
        public void DrawText(string spriteName, System.Drawing.Rectangle rect, Rectanglei uv)
        {
            VboDrawContents.Clear();
            VboUvContents.Clear();
            Rectanglei spriteRect = uv;

            VboDrawContents.AddRange(GLUtility.MakeVboData(rect));
            VboUvContents.AddRange(GLUtility.MakeVboData(spriteRect));

            VertexCount += 12;
        }
        public void FinishText(int id, Vector2 size)
        {
            // Create VBO for the batch
            //
            int vboDrawId = GL.GenBuffer();
            int vboUvId = GL.GenBuffer();
            float[] spriteDrawVerts = VboDrawContents.ToArray();
            float[] spriteUvVerts = VboUvContents.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteDrawVerts.Length),
                spriteDrawVerts,
                BufferUsageHint.StreamDraw
                );

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                new IntPtr(sizeof(float) * spriteUvVerts.Length),
                spriteUvVerts,
                BufferUsageHint.StreamDraw
                );

            // Set up shader program
            //
            GL.UseProgram(GlProgramId);

            GL.Uniform2(
                GlCanvasResolutionUniformId,
                (float)OwnerController.TargetResolution.Width,
                (float)OwnerController.TargetResolution.Height
                );

            GL.Uniform2(GlUvMapResolutionUniformId, size);

            // Bind the atlas
            //
            GL.BindTexture(
                TextureTarget.Texture2D,
                id
                );

            // Assign draw attrib
            //
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboDrawId);
            GL.VertexAttribPointer(
                0,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );

            // Assign UV attrib
            //
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvId);
            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );


            // Draw now!
            //
            GL.DrawArrays(BeginMode.Triangles, 0, VertexCount);

            // Detach and destroy VBOs
            //
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.DeleteBuffer(vboDrawId);
            GL.DeleteBuffer(vboUvId);
        }

    }
}
