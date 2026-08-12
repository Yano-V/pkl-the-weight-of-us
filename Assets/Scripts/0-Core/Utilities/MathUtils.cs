namespace ProjLimbo
{
    public static class MathUtils
    {
        public static float WrapAngle180(float angle)
        {
            angle = (angle + 180f) % 360f;
            if (angle < 0f)
            {
                angle += 360f;
            }
            return angle - 180f;
        }
    }    
}