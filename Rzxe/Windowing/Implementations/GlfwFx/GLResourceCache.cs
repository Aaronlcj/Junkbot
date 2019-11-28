﻿using Oddmatics.Rzxe.Game;
using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oddmatics.Rzxe.Windowing.Implementations.GlfwFx
{
    internal class GLResourceCache : IDisposable
    {
        private Dictionary<string, GLSpriteAtlas> Atlases { get; set; }

        private bool Disposing { get; set; }

        private IGameEngineParameters EngineParameters { get; set; }

        private Dictionary<string, uint> ShaderPrograms { get; set; }

        
        public GLResourceCache(IGameEngineParameters engineParameters)
        {
            Atlases = new Dictionary<string, GLSpriteAtlas>();
            EngineParameters = engineParameters;
            ShaderPrograms = new Dictionary<string, uint>();
        }


        public GLSpriteAtlas GetAtlas(string atlasName, int type = 0)
        {
            if (Disposing)
            {
                throw new ObjectDisposedException("GLFW Resource Cache");
            }

            string atlasNameLower = atlasName.ToLower();

            if (Atlases.ContainsKey(atlasNameLower))
            {
                return Atlases[atlasNameLower];
            }
            else
            {
                GLSpriteAtlas newAtlas;
                if (type == 1)
                {
                    newAtlas = GLSpriteAtlas.FromGif(atlasName);
                }
                else
                {
                    newAtlas = GLSpriteAtlas.FromFileSet(
                        string.Format(
                            "{0}\\Atlas\\{1}",
                            EngineParameters.GameContentRoot,
                            atlasName
                        )
                    );
                }

                Atlases.Add(newAtlas.Name, newAtlas);
                 
                return newAtlas;
            }
        }

        public void Dispose()
        {
            if (Disposing)
            {
                throw new ObjectDisposedException("GLFW Resource Cache");
            }

            Disposing = true;

            // Delete all shader programs
            //
            foreach (uint glProgramId in ShaderPrograms.Values)
            {
                GL.DeleteProgram(glProgramId);
            }

            ShaderPrograms.Clear();
            ShaderPrograms = null;

            // Delete all atlases
            //
            foreach (GLSpriteAtlas atlas in Atlases.Values)
            {
                atlas.Dispose();
            }

            Atlases.Clear();
            Atlases = null;
        }

        public uint GetShaderProgram(string programName)
        {
            if (Disposing)
            {
                throw new ObjectDisposedException("GLFW Resource Cache");
            }

            // If the shader program has previously been loaded, return its GL ID
            //
            if (ShaderPrograms.ContainsKey(programName))
            {
                return ShaderPrograms[programName];
            }

            // We reached here, program has not yet been cached, need to compile it
            // from sources
            //
            string fragmentSource = File.ReadAllText(
                string.Format(
                    "{0}{2}Shaders{2}OpenGL{2}{1}{2}fragment.glsl",
                    EngineParameters.GameContentRoot,
                    programName,
                    Path.DirectorySeparatorChar
                )
            );
            string vertexSource = File.ReadAllText(
                string.Format(
                    "{0}{2}Shaders{2}OpenGL{2}{1}{2}vertex.glsl",
                    EngineParameters.GameContentRoot,
                    programName,
                    Path.DirectorySeparatorChar
                )
            );
            uint compiledProgramId = GLUtility.CompileShaderProgram(
                vertexSource,
                fragmentSource
            );

            ShaderPrograms.Add(programName, compiledProgramId);

            return compiledProgramId;
        }
    }
}
