using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HalJsonNet.Utility;

namespace HalJsonNet
{
    static class AttributeConfigurationResolver
    {
        private static readonly ThreadSafeCache<Type, HalJsonTypeConfiguration> Cache =
            new ThreadSafeCache<Type, HalJsonTypeConfiguration>();

        
        public static HalJsonTypeConfiguration GetConfigurationOrNull(Type t)
        {
            var value = Cache.GetOrAdd(t, Resolve);
            return value == null ? null : value.Clone();
        }

        static HalJsonTypeConfiguration Resolve(Type t)
        {
            var found = false;
            var rv =
                (HalJsonTypeConfiguration)
                    Activator.CreateInstance(typeof (HalJsonTypeConfiguration<>).MakeGenericType(t));

            foreach (var prop in t.GetProperties())
            {
                var embedded = prop.GetCustomAttributes().OfType<HalJsonEmbeddedAttribute>().FirstOrDefault();
                if (embedded != null)
                {
                    found = true;
                    rv.Embed(embedded.Name, CreateGetter(t, prop));
                    if (embedded.HideProperty)
                        rv.HideProperty(prop);
                }
                var link = prop.GetCustomAttributes().OfType<HalJsonLinkAttribute>().FirstOrDefault();
                if (link != null)
                {
                    found = true;
                    rv.Link(link.Name, new Link(LinkGetter(CreateGetter(t, prop), link.Link), link.Templated));
                    if (link.HideProperty)
                        rv.HideProperty(prop);
                }
            }
            foreach (var classLink in t.GetCustomAttributes().OfType<HalJsonLinkAttribute>())
            {
                found = true;
                rv.Link(classLink.Name, classLink.Link, classLink.Templated);
            }
            

            return found ? rv : null;
        }

        private static Func<object, string> LinkGetter(Func<object, object> realGetter, string template)
        {
            if (template.Contains("{0}"))
                return target => string.Format(template, realGetter(target));
            return _ => template;
        }


        static Func<object, object> CreateGetter(Type t, PropertyInfo property)
        {
            var arg = Expression.Parameter(typeof(object), "obj");
            var target = Expression.Convert(arg, t);
            var getProp = Expression.Convert(Expression.Call(target, property.GetGetMethod(true)), typeof (object));
            return Expression.Lambda<Func<object, object>>(getProp, arg).Compile();
        }

    }
}
