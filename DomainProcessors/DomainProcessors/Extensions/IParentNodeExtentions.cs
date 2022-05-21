using AngleSharp.Dom;
using DomainProcessors.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DomainProcessors.Extensions
{
    public static class IParentNodeExtentions
    {
        public static IHtmlCollection<IElement> QuerySelectorAllOrThrow(this IParentNode parentNode, String selectors)
        {
            IHtmlCollection<IElement>? elements = parentNode.QuerySelectorAll(selectors);
            if (elements == null || elements.Length == 0)
            {
                throw new IParentNodeProcessingException($"Failed to find elemenets with query selector [{selectors}]");
            }
            return elements;
        }
        
        public static IElement QuerySelectorOrThrow(this IParentNode parentNode, String selectors)
        {
            IElement? element = parentNode.QuerySelector(selectors);
            if (element == null)
            {
                throw new IParentNodeProcessingException($"Failed to find elemenet with query selector [{selectors}]");
            }
            return element;
        }
    }
}
