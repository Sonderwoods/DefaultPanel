using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

/*
 *  HIGHLY based on code by Long Nguyens ScriptSync (and he had some help from David Rutten), Thanks both of you.
 */

namespace DefaultPanel
{
    public partial class GhcDefaultPanel : GH_Component
    {
        protected class GhcDefaultPanelAttributes : GH_Attributes<GhcDefaultPanel>
        {
            public GhcDefaultPanelAttributes(GhcDefaultPanel owner) : base(owner)
            {


            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {

                (Owner as GhcDefaultPanel).UpdatePanels();
                return GH_ObjectResponse.Handled;
            }


            protected override void Layout()
            {
                //Size size = GH_FontServer.MeasureString("DefaultPanel", GH_FontServer.Standard) + new Size(30, 150);
                //Bounds = new RectangleF(
                //    Pivot.X - 0.5f * size.Width,
                //    Pivot.Y - 0.5f * size.Height,
                //    size.Width,
                //    size.Height);

                Size size = new Size(130, 18);
                Bounds = new RectangleF(
                    Pivot.X - 0.5f * size.Width,
                    Pivot.Y - 0.5f * size.Height,
                    size.Width,
                    size.Height
                    );

            }


            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                if (channel == GH_CanvasChannel.Wires)
                {
                    var targetComponent = Owner.OnPingDocument().FindObject(Owner.targetPanelComponentGuid, true);
                    var sourceComponent = Owner.OnPingDocument().FindObject(Owner.sourcePanelComponentGuid, true);

                    

                    if (targetComponent != null)
                    {
                        RectangleF myBounds = Owner.Attributes.Bounds;
                        PointF myPoint = new PointF(myBounds.X + myBounds.Width, myBounds.Y + myBounds.Height / 2);

                        RectangleF targetBounds = targetComponent.Attributes.Bounds;
                        PointF targetCenter = new PointF(targetBounds.X, targetBounds.Y + targetBounds.Height / 2);

                        graphics.DrawLine(
                            new Pen(Color.Black, 3f) { DashCap = DashCap.Round, DashPattern = new[] { 1f, 0.5f } },
                            myPoint,
                            targetCenter);
                    }
                        

                    if (sourceComponent != null)
                    {
                        RectangleF myBounds = Owner.Attributes.Bounds;
                        PointF myPoint = new PointF(myBounds.X, myBounds.Y + myBounds.Height / 2);

                        RectangleF targetBounds = sourceComponent.Attributes.Bounds;
                        PointF sourceCenter = new PointF(targetBounds.X + targetBounds.Width, targetBounds.Y + targetBounds.Height / 2);

                        graphics.DrawLine(
                            new Pen(Color.Black, 2f) { DashCap = DashCap.Round, DashPattern = new[] { 1f, 0.5f } },
                            myPoint,
                            sourceCenter);
                    }

                }
                else if (channel == GH_CanvasChannel.Objects)
                {
                    GH_Palette ghPalette = GH_Palette.Hidden;
                    if (Owner.RuntimeMessageLevel == GH_RuntimeMessageLevel.Warning) ghPalette = GH_Palette.Warning;
                    else if (Owner.RuntimeMessageLevel == GH_RuntimeMessageLevel.Error) ghPalette = GH_Palette.Error;

                    GH_Capsule ghTextCapsule = GH_Capsule.CreateTextCapsule(
                        GH_Convert.ToRectangle(Bounds),
                        GH_Convert.ToRectangle(Bounds),
                        ghPalette,
                        "--> DefaultPanel -->",
                        GH_FontServer.Standard,
                        GH_Orientation.horizontal_center,
                        4,
                        0);

                    ghTextCapsule.Render(graphics, null, Selected, Owner.Locked, false);
                  
                }
            }
        }
    }
}