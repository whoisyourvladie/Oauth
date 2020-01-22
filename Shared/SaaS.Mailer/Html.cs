using RazorEngine.Text;

namespace SaaS.Mailer
{
    public static class Html
    {
        public static RawString Raw(string value)
        {
            return new RawString(value);
        }
    }
}
