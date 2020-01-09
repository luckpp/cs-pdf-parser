using System.Text;
using iTextSharp.text.pdf.parser;

namespace FordParser.Parser
{
    class FordExtractionCustomStrategy : ITextExtractionStrategy
    {
        private readonly StringBuilder _result;
        private StringBuilder _token;
        
        public FordExtractionCustomStrategy()
        {
            _result = new StringBuilder();
        }

        public void BeginTextBlock()
        {
            _token = new StringBuilder();
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            var text = renderInfo.GetText();
            _token.Append(text);
            _token.Append(" ");
        }

        public void EndTextBlock()
        {
            _result.AppendLine(_token.ToString());
            _token = new StringBuilder();
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            ;
        }

        public string GetResultantText()
        {
            return _result.ToString().Trim();
        }
    }
}
