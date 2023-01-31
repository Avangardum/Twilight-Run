using System.Collections.Generic;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class QuadBox : MonoBehaviour
    {
        [field: SerializeField] public Renderer LeftQuadRenderer { get; private set; }
        [field: SerializeField] public Renderer RightQuadRenderer { get; private set; }
        [field: SerializeField] public Renderer TopQuadRenderer { get; private set; }
        [field: SerializeField] public Renderer BottomQuadRenderer { get; private set; }
        [field: SerializeField] public Renderer FrontQuadRenderer { get; private set; }
        [field: SerializeField] public Renderer BackQuadRenderer { get; private set; }
        public List<Renderer> Renderers { get; private set; }

        private Vector3 _previousScale;

        private void RefreshMaterialTiling()
        {
            var scale = transform.localScale;
            var xScale = scale.x;
            var yScale = scale.y;
            var zScale = scale.z;
            
            LeftQuadRenderer.material.mainTextureScale = new Vector2(zScale, yScale);
            RightQuadRenderer.material.mainTextureScale = new Vector2(zScale, yScale);
            TopQuadRenderer.material.mainTextureScale = new Vector2(xScale, zScale);
            BottomQuadRenderer.material.mainTextureScale = new Vector2(xScale, zScale);
            FrontQuadRenderer.material.mainTextureScale = new Vector2(xScale, yScale);
            BackQuadRenderer.material.mainTextureScale = new Vector2(xScale, yScale);
            
            _previousScale = scale;
        }

        private void Awake()
        {
            Renderers = new List<Renderer>
            {
                LeftQuadRenderer,
                RightQuadRenderer,
                TopQuadRenderer,
                BottomQuadRenderer,
                FrontQuadRenderer,
                BackQuadRenderer
            };
            RefreshMaterialTiling();
            transform.hasChanged = false;
        }

        private void Update()
        {
            if (transform.hasChanged && transform.localScale != _previousScale)
            {
                RefreshMaterialTiling();
                transform.hasChanged = false;
            }
        }
    }
}