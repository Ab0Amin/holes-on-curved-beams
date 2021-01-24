using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tekla.Structures.Model;
using t3d=Tekla.Structures.Geometry3d;
using Tekla.Structures.Geometry3d;
using System.Collections;
using tsmui=Tekla.Structures.Model.UI;
using Tekla.Structures.Model.UI;

namespace holes_on_curved_beams
{
    public partial class Form1 : Form
    {
        public Beam check_with_beam(t3d.Point start_point, t3d.Point end_point)
        {
            Beam toe_plate = new Beam();
            toe_plate.StartPoint = start_point;
            toe_plate.EndPoint = end_point;
            toe_plate.Profile.ProfileString = "ROD100";
            toe_plate.Position.Depth = Position.DepthEnum.FRONT;
            toe_plate.Position.Plane = Position.PlaneEnum.RIGHT;
            toe_plate.Position.Rotation = Position.RotationEnum.TOP;
            toe_plate.Insert();
            return toe_plate;
        }
      
       
        public void get_circle_center(PolyBeam main_part ,out t3d.Point center,out double radius)
        {
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            CoordinateSystem mainPart_coordinat = main_part.GetCoordinateSystem();
            if (main_part.Position.Rotation == Position.RotationEnum.BACK || main_part.Position.Rotation == Position.RotationEnum.FRONT)
            {
                mainPart_coordinat.AxisY = mainPart_coordinat.AxisX.Cross(mainPart_coordinat.AxisY);
            }

            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(mainPart_coordinat));
            ArrayList points = main_part.GetCenterLine(true);
            //  getCircleCenter(points[0] as t3d.Point, points[points.Count-1] as t3d.Point, points[(int)(points.Count/2)] as t3d.Point,out a,out b,out c);
            t3d.Point point_1 = points[(int)(points.Count / 2)-1] as t3d.Point;
            t3d.Point point_2 = points[(int)(points.Count / 2)] as t3d.Point;
            t3d.Point point_3 = points[((int)(points.Count / 2)+1)] as t3d.Point;
            Line _1_line = new Line(point_1, point_2);
            Line _2_line = new Line(point_3, point_2);
            t3d.Point mid_of_1line = new t3d.Point((point_1.X + point_2.X) / 2, (point_1.Y + point_2.Y) / 2, (point_1.Z + point_2.Z) / 2);
            t3d.Point mid_of_2line = new t3d.Point((point_3.X + point_2.X) / 2, (point_3.Y + point_2.Y) / 2, (point_3.Z + point_2.Z) / 2);
            t3d.Vector fist_vector = new Vector(_1_line.Direction);
            t3d.Vector scnd_vector = new Vector(_2_line.Direction);
            
            t3d.Line first_intersected_line = new Line(mid_of_1line, fist_vector.Cross(new t3d.Vector(0, 0, 1)));
            t3d.Line sec_intersected_line = new Line(mid_of_2line, scnd_vector.Cross(new t3d.Vector(0, 0, 1)));
            LineSegment final = Intersection.LineToLine(first_intersected_line, sec_intersected_line);
            // radius
            radius = Math.Sqrt(Math.Pow(point_1.X - final.Point1.X, 2) + Math.Pow(point_1.Y - final.Point1.Y, 2));
            center = final.Point1;
        }
        public double angleOfCurve(double distance,double radius)
        {
            double angle = distance * 360 / (2 * Math.PI * radius);
            return angle;
        }

        public BoltArray insert_hole(PolyBeam mainPart, t3d.Point point1,t3d.Point point2 , double size , double angle)
        {
            BoltArray hole = new BoltArray();
            hole.PartToBeBolted = mainPart;
            hole.PartToBoltTo = mainPart;
            hole.FirstPosition = point1;
            hole.SecondPosition = point2;
            hole.Position.Rotation = Position.RotationEnum.BACK;
            hole.Position.RotationOffset = angle;
            hole.Position.Rotation = Position.RotationEnum.BACK;
            hole.Position.Depth = Position.DepthEnum.MIDDLE;
            hole.Position.Plane = Position.PlaneEnum.MIDDLE;
            hole.AddBoltDistX(0);
            hole.AddBoltDistY(0);
            hole.Tolerance = 0;
            hole.BoltSize = size;

            hole.Hole1 = false;
            hole.Hole2 = false;
            hole.Hole3 = false;
            hole.Hole4 = false;
            hole.Hole5 = false;
            hole.Washer1 = false;
            hole.Washer2 = false;
            hole.Washer3 = false;
            hole.Nut1 = false;
            hole.Nut2 = false;

            hole.Insert();
            return hole;
        }

        public void conver_coordinates_center_point(t3d.Point center, t3d.Point point1,out t3d.Point center1, out t3d.Point point2 )
        {
            TransformationPlane transformationPlane = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            t3d.Vector vector = new Vector(-center.X + point1.X, -center.Y + point1.Y, -center.Z + point1.Z);
         
            t3d.Vector vectory = vector.Cross(new Vector(0, 0, -1));
            vector.Normalize();
            vectory.Normalize();
            TransformationPlane transformation = new TransformationPlane(center, vector, vectory);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformation);

            center1 = transformation.TransformationMatrixToLocal.Transform
                   (transformationPlane.TransformationMatrixToGlobal.Transform(center));
            point2 = transformation.TransformationMatrixToLocal.Transform
                (transformationPlane.TransformationMatrixToGlobal.Transform(point1));
        }

        public void calculate2HolesCoordiates(t3d.Point center, double inner_radius,double outer_radius,double curve_angle,out t3d.Point point_dis_in,out t3d.Point point_dis_out)
        {
            //calculate required inner point
            double y_point = inner_radius * Math.Sin(curve_angle * Math.PI / 180);
            double x_point = inner_radius * Math.Cos(curve_angle * Math.PI / 180);
             point_dis_in = new t3d.Point(center.X + x_point, center.Y + y_point, center.Z);
           

            //calculate required out point
            y_point = outer_radius * Math.Sin(curve_angle * Math.PI / 180);
            x_point = outer_radius * Math.Cos(curve_angle * Math.PI / 180);
            point_dis_out = new t3d.Point(center.X + x_point, center.Y + y_point, center.Z);

             }


        public void run()
        {
            try
            {
                //parameters
                t3d.Point center;
                double radius;
                double height = 0;
                double width = 0;
                int rotaion = -1;
           
                if (cm_rotation.SelectedIndex == 1)
                {
                    rotaion = 1;
                }
                double curve_angle = 0;
                double holeSize = int.Parse(tx_size.Text);

                //picking one part and one point
                //part
                PolyBeam main_beam = new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART) as PolyBeam;
                //set coordiates
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
                CoordinateSystem mainPart_coordinat = main_beam.GetCoordinateSystem();
                if (main_beam.Position.Rotation == Position.RotationEnum.BACK || main_beam.Position.Rotation == Position.RotationEnum.FRONT)
                {
                    mainPart_coordinat.AxisY = mainPart_coordinat.AxisX.Cross(mainPart_coordinat.AxisY);
                    main_beam.GetReportProperty("WIDTH", ref height);
                    main_beam.GetReportProperty("HEIGHT", ref width);

                }
                else
                {
                    main_beam.GetReportProperty("HEIGHT", ref height);
                    main_beam.GetReportProperty("WIDTH", ref width);

                }

                TransformationPlane transformationPlane = new TransformationPlane(mainPart_coordinat);
                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);
                //pick point
                ArrayList point = new Picker().PickPoints(Picker.PickPointEnum.PICK_ONE_POINT);
                t3d.Point first_point = point[0] as t3d.Point;
                first_point = new t3d.Point(first_point.X, first_point.Y, 0);


                // get proprties of curved beam
                get_circle_center(main_beam, out center, out radius);

                double inner_radius = radius - height / 2;
                double outer_radius = radius + height / 2;


                // get angle of inner curve
                curve_angle = angleOfCurve(double.Parse(tx_dis.Text) * -1, radius);

                // convert center and picking point to new local coorditaes
                conver_coordinates_center_point(center, first_point, out center, out first_point);

                //calculate required inner point
                t3d.Point point1_dis;
                t3d.Point point2_dis;
                calculate2HolesCoordiates(center, radius, outer_radius, curve_angle, out point1_dis, out point2_dis);

                //// insert hole  curve
                BoltArray hole1 = insert_hole(main_beam, new t3d.Point(point1_dis.X, point1_dis.Y, point1_dis.Z + width/2 * rotaion),
                      new t3d.Point(point1_dis.X+10, point1_dis.Y, point1_dis.Z +width/2 * rotaion)
                      , holeSize, 0);


                //calculate required out point
                // insert hole  curve
                /*  BoltArray hole2 = insert_hole(main_beam, point2_dis,
                      new t3d.Point(point2_dis.X, point2_dis.Y, point2_dis.Z + 10)
                      , holeSize, 0);*/
                // get bol vector

                LineSegment secHole = new LineSegment(new t3d.Point(hole1.FirstPosition.X, hole1.FirstPosition.Y, hole1.FirstPosition.Z + 1000),
                 new t3d.Point(hole1.FirstPosition.X, hole1.FirstPosition.Y, hole1.FirstPosition.Z - 1000));
                
               


                Solid checkIntesection = main_beam.GetSolid();
                ArrayList intersectedPoints = checkIntesection.Intersect(secHole);
                // loop
                while (intersectedPoints.Count > 0)
                {
                    conver_coordinates_center_point(center, point1_dis, out center, out point1_dis);
                    calculate2HolesCoordiates(center, radius, outer_radius, curve_angle, out point1_dis, out point2_dis);
                    //// insert hole  curve
                     hole1 = insert_hole(main_beam, new t3d.Point(point1_dis.X, point1_dis.Y, point1_dis.Z + width / 2*rotaion),
                          new t3d.Point(point1_dis.X + 10, point1_dis.Y, point1_dis.Z + width / 2 * rotaion)
                          , holeSize, 0);


                    //calculate required out point
                    // insert hole  curve
                    /*  hole2 = insert_hole(main_beam, point2_dis,
                          new t3d.Point(point2_dis.X, point2_dis.Y, point2_dis.Z + 10)
                          , holeSize, curve_angle);*/

                    // firstHole = new LineSegment(hole1.FirstPosition, hole1.SecondPosition);
                     secHole = new LineSegment(new t3d.Point(hole1.FirstPosition.X, hole1.FirstPosition.Y, hole1.FirstPosition.Z + 1000),
                   new t3d.Point(hole1.FirstPosition.X, hole1.FirstPosition.Y, hole1.FirstPosition.Z - 1000));

                    intersectedPoints = checkIntesection.Intersect(secHole);


                }
                if (intersectedPoints.Count == 0)
                {
                    hole1.Delete();
                    //hole2.Delete();
                }

                model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());


                model.CommitChanges();
            }
            catch (Exception)
            {


            }
        }
        Model model = new Model();

        public Form1()
        {
            InitializeComponent();
            cm_rotation.SelectedIndex = 0;
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            run();
        }


        #region old code
        //          try
        //            {
        //                //parameters
        //                t3d.Point center;
        //        double radius;
        //        double height = 0;
        //        double curve_angle = 0;

        //        //picking one part and one point
        //        //part
        //        PolyBeam main_beam = new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART) as PolyBeam;
        //        //set coordiates
        //        model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        //                CoordinateSystem mainPart_coordinat = main_beam.GetCoordinateSystem();
        //                if (main_beam.Position.Rotation == Position.RotationEnum.BACK || main_beam.Position.Rotation == Position.RotationEnum.FRONT)
        //                {
        //                    mainPart_coordinat.AxisY = mainPart_coordinat.AxisX.Cross(mainPart_coordinat.AxisY);
        //                    main_beam.GetReportProperty("WIDTH", ref height);
        //                }
        //                else
        //                {
        //                    main_beam.GetReportProperty("HEIGHT", ref height);
        //                }

        //TransformationPlane transformationPlane = new TransformationPlane(mainPart_coordinat);
        //model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);
        ////pick point
        //ArrayList point = new Picker().PickPoints(Picker.PickPointEnum.PICK_ONE_POINT);
        //t3d.Point first_point = point[0] as t3d.Point;
        //first_point = new t3d.Point(first_point.X, first_point.Y, 0);

        //// get proprties of curved beam
        //get_circle_center(main_beam, out center, out radius);

        //double inner_radius = radius - height / 2;
        //double outer_radius = radius + height / 2;


        //// get angle of inner curve
        //curve_angle = angleOfCurve(double.Parse(tx_dis.Text), radius);

        //// convert center and picking point to new local coorditaes
        ////t3d.Vector vector = new Vector(-center.X + first_point.X, -center.Y + first_point.Y, -center.Z + first_point.Z);
        ////TransformationPlane transformation = new TransformationPlane(center, vector, vector.Cross(new Vector(0, 0, 1)));
        ////model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformation);

        ////center = transformation.TransformationMatrixToLocal.Transform
        ////       (transformationPlane.TransformationMatrixToGlobal.Transform(center));
        ////first_point = transformation.TransformationMatrixToLocal.Transform
        ////    (transformationPlane.TransformationMatrixToGlobal.Transform(first_point));
        //conver_coordinates_center_point(center, first_point, out center, out first_point);

        ////calculate required inner point
        //t3d.Point point1_dis;
        //t3d.Point point2_dis;
        //calculate2HolesCoordiates(center, inner_radius, outer_radius, curve_angle, out point1_dis, out point2_dis);

        ////double y_point = inner_radius * Math.Sin(curve_angle * Math.PI / 180);
        ////double x_point = inner_radius * Math.Cos(curve_angle * Math.PI / 180);
        ////t3d.Point point1_dis = new t3d.Point(center.X + x_point, center.Y + y_point, center.Z);
        ////// insert hole  curve
        //insert_hole(main_beam, new t3d.Point(point1_dis.X, point1_dis.Y, point1_dis.Z),
        //    new t3d.Point(point1_dis.X, point1_dis.Y, point1_dis.Z + 10)
        //    , int.Parse(tx_size.Text), curve_angle);


        ////calculate required out point
        ////y_point = outer_radius * Math.Sin(curve_angle * Math.PI / 180);
        ////x_point = outer_radius * Math.Cos(curve_angle * Math.PI / 180);
        ////point1_dis = new t3d.Point(center.X + x_point, center.Y + y_point, center.Z);

        //// insert hole  curve
        //insert_hole(main_beam, new t3d.Point(point2_dis.X, point2_dis.Y, point2_dis.Z),
        //    new t3d.Point(point2_dis.X, point2_dis.Y, point2_dis.Z + 10)
        //    , int.Parse(tx_size.Text), curve_angle);

        //model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());


        //model.CommitChanges();
        //            }
        //            catch (Exception)
        //{


        //}
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
