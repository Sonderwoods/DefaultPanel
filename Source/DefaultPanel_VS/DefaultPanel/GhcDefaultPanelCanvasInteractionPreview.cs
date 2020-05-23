using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Point = System.Drawing.Point;

/*
 *  HIGHLY based on code by Long Nguyens ScriptSync (and he had some help from David Rutten), Thanks both of you.
 */


namespace DefaultPanel
{
    public partial class GhcDefaultPanel : GH_Component
    {

        protected class GhcDefaultPanelCanvasInterationPreview : GH_AbstractInteraction
        {
            internal readonly GhcDefaultPanel ghcDefaultPanel;


            public GhcDefaultPanelCanvasInterationPreview(GH_Canvas ghCanvas, GH_CanvasMouseEvent mouseEvent,
                GhcDefaultPanel ghcDefaultPanel)
                : base(ghCanvas, mouseEvent, true)
            {
                this.ghcDefaultPanel = ghcDefaultPanel;
                m_canvas.CanvasPostPaintObjects += canvasPostPaintObjectsX;
            }


            public override void Destroy()
            {
                //m_canvas.CanvasPostPaintObjects -= canvasPostPaintObjects;
            }

            public void Kill()
            {
                //ghcDefaultPanel.ExpireSolution(true);
                m_canvas.CanvasPostPaintObjects -= canvasPostPaintObjectsX;
                

            }


            public override GH_ObjectResponse RespondToMouseMove(GH_Canvas ghCanvas, GH_CanvasMouseEvent e)
            {
                //ghCanvas.Refresh();
                dynamic targetPanelComponent = ghcDefaultPanel.OnPingDocument().FindObject(ghcDefaultPanel.TargetPanelComponentGuid, true);
                if (targetPanelComponent == null)
                {
                    ghcDefaultPanel.TargetPanelComponentGuid = Guid.Empty;
                    ghcDefaultPanel.ExpireSolution(true);
                }
                //return GH_ObjectResponse.Handled;
                return GH_ObjectResponse.Release;
            }


            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas ghCanvas, GH_CanvasMouseEvent e)
            {
                return GH_ObjectResponse.Release;
            }


            public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
            {
                return GH_ObjectResponse.Release;
            }


            private void canvasPostPaintObjectsX(GH_Canvas ghCanvas)
            {
                try
                {
                    var tp = ghcDefaultPanel.OnPingDocument().FindObject(ghcDefaultPanel.targetPanelComponentGuid, true);
                    RectangleF tb = tp.Attributes.Bounds;
                }
                catch (NullReferenceException)
                {
                    Kill();
                    return;
                }


                var targetPanel = ghcDefaultPanel.OnPingDocument().FindObject(ghcDefaultPanel.targetPanelComponentGuid, true);
                RectangleF targetBounds = targetPanel.Attributes.Bounds;
                PointF targetPoint = new PointF(targetBounds.X, targetBounds.Y + targetBounds.Height / 2);

                ghCanvas.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                RectangleF myBounds = ghcDefaultPanel.Attributes.Bounds;
                PointF myPoint = new PointF(myBounds.X + myBounds.Width, myBounds.Y + myBounds.Height / 2);


                ghCanvas.Graphics.DrawLine(
                    new Pen(Color.Gray, 3f) { DashCap = DashCap.Round, DashPattern = new[] { 3f, 0.25f } },
                    myPoint,
                    targetPoint);
            }
        }
    }
}
