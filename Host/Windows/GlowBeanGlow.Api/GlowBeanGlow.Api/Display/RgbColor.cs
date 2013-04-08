namespace GlowBeanGlow.Api.Display
{
    public class RgbColor
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public byte[] GetBytes()
        {
            return new byte[] { Red, Green, Blue};
        }
    }
}
