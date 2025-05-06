using UnityEngine;

namespace Seino.Utils
{
    public class Line2D
    {
        /// <summary>
        /// 共线
        /// </summary>
        public const int COLINE = 0;

        /// <summary>
        /// 相交
        /// </summary>
        public const int CROSS = 1;

        /// <summary>
        /// 平行线
        /// </summary>
        public const int PARALLEL = 2;

        /// <summary>
        /// 无相交
        /// </summary>
        public const int NOT_CROSS = 3;
        
        private double EPS = 1e-4;

        public Vector3 point1;
        public Vector3 point2;
        
        private float A;
        private float B;
        private float C;

        public Line2D(Vector3 point1, Vector3 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
            CalcCoefficient();
        }

        /// <summary>
        /// 计算两点组成的直线的系数（Ax+By+C=0）
        /// </summary>
        private void CalcCoefficient()
        {
            this.A = this.point2.y - this.point1.y;
            this.B = this.point1.x - this.point2.x;
            this.C = this.point2.x * this.point1.y - this.point1.x * this.point2.y;
        }

        /// <summary>
        /// 检查是否交叉(当前线的两点，其他线的两点)
        /// </summary>
        /// <param name="sp1"></param>
        /// <param name="ep1"></param>
        /// <param name="sp2"></param>
        /// <param name="ep2"></param>
        /// <returns></returns>
        private bool CheckCross(Vector3 sp1, Vector3 ep1, Vector3 sp2, Vector3 ep2)
        {
            if (Mathf.Max(sp1.x, ep1.x) < Mathf.Min(sp2.x, ep2.x)) return false;
            if (Mathf.Min(sp1.x, ep1.x) > Mathf.Max(sp2.x, ep2.x)) return false;
            if (Mathf.Max(sp1.y, ep1.y) < Mathf.Min(sp2.y, ep2.y)) return false;
            if (Mathf.Min(sp1.y, ep1.y) > Mathf.Max(sp2.y, ep2.y)) return false;

            Vector3 vectorA = sp1 - sp2;
            Vector3 vectorB = ep2 - sp2;
            Vector3 vectorC = ep2 - sp2;
            Vector3 vectorD = ep1 - sp2;
            double temp1 = (vectorA.x * vectorB.y - vectorA.y * vectorB.x) * (vectorC.x * vectorD.y - vectorC.y * vectorD.x);

            vectorA = sp2 - sp1;
            vectorB = ep1 - sp1;
            vectorC = ep1 - sp1;
            vectorD = ep2 - sp1;
            double temp2 = (vectorA.x * vectorB.y - vectorA.y * vectorB.x) * (vectorC.x * vectorD.y - vectorC.y * vectorD.x);

            return (temp1 >= 0) && (temp2 >= 0);
        }


        private bool IsDoubleEqualZero(float data)
        {
            return Mathf.Abs(data) <= EPS;
        }

        /// <summary>
        /// 交点计算
        /// </summary>
        /// <param name="otherLine">其他要比较的线</param>
        /// <param name="intersectantPoint">交点</param>
        /// <returns>两条线的情况</returns>
        public int Intersection(Line2D otherLine, out Vector2 intersectantPoint)
        {
            intersectantPoint = Vector2.zero;
            //检查是否相交
            if (!CheckCross(this.point1, this.point2, otherLine.point1, otherLine.point2))
            {
                return NOT_CROSS;
            }

            //检查是否平行
            if (IsDoubleEqualZero(this.A * otherLine.B - this.B * otherLine.A))
            {
                if (IsDoubleEqualZero((this.A + this.B) * otherLine.C - (otherLine.A + otherLine.B) * this.C))
                {
                    return COLINE;
                }
                return PARALLEL;
            }
            
            //    C1*B2-B1*C2           A1*C2-C1*A2
            // X=-------------       Y=--------------
            //    B1*A2-A1*B2           B1*A2-A1*B2
            intersectantPoint.x = (otherLine.B * this.C - this.B * otherLine.C) / (otherLine.A * this.B - this.A * otherLine.B);
            intersectantPoint.y = (this.A * otherLine.C - otherLine.A * this.C) / (otherLine.A * this.B - this.A * otherLine.B);

            return CROSS;
        }
    }
}