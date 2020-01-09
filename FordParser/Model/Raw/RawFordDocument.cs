using System;
using System.Collections.Generic;
using System.Linq;

namespace FordParser.Model.Raw
{
    public class RawFordDocument
    {
        public RawFordDocument(string name, IEnumerable<RawFordPage> pages)
        {
            Name = name;

            var orderedPages = pages.OrderBy(page => page.Number).ToArray();
            Pages = orderedPages;
            UnifiedPage = Unify(orderedPages);
        }

        public string Name { get; }

        public IEnumerable<RawFordPage> Pages { get; }

        public UnifiedRawFordPage UnifiedPage { get; }

        private UnifiedRawFordPage Unify(IReadOnlyCollection<RawFordPage> pages)
        {
            var allLines = new List<string>();
            var pageCount = pages.Count;

            foreach (var page in pages)
            {
                var pageNumber = page.Number;
                var lines = page.Lines;

                if (IsSinglePage(pageNumber, pageCount))
                {
                    allLines.AddRange(lines);
                }
                else if (IsStartPage(pageNumber, pageCount))
                {
                    var content = lines
                        .TakeWhile(line => !line.StartsWith("Page:  "));

                    allLines.AddRange(content);
                }
                else if (IsMiddlePage(pageNumber, pageCount))
                {
                    var content = lines
                        .SkipWhile(line => !line.StartsWith("Material Weight"))
                        .Skip(1)
                        .TakeWhile(line => !line.StartsWith("Page:  "));

                    allLines.AddRange(content);
                }
                else if (IsEndPage(pageNumber, pageCount))
                {
                    var content = lines
                        .SkipWhile(line => !line.StartsWith("Material Weight"))
                        .Skip(1);

                    allLines.AddRange(content);
                }
            }

            return new UnifiedRawFordPage(allLines.ToArray());
        }

        private bool IsSinglePage(int pageNumber, int pageCount)
        {
            return pageNumber == 1 && pageCount == 1;
        }

        private bool IsStartPage(int pageNumber, int pageCount)
        {
            return pageNumber == 1 && pageNumber < pageCount;
        }

        private bool IsMiddlePage(int pageNumber, int pageCount)
        {
            return pageNumber > 1 && pageNumber < pageCount;
        }

        private bool IsEndPage(int pageNumber, int pageCount)
        {
            return pageNumber > 1 && pageNumber == pageCount;
        }
    }
}
