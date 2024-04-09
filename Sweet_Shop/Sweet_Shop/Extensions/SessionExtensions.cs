using Newtonsoft.Json;

namespace Sweet_Shop.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static bool IsCustomerIdExistsInSession(this ISession session)
        {
            return session.GetString("CustomerID") != null;
        }
        public static bool IsAdminIdExistsInSession(this ISession session)
        {
            return session.GetString("AdminID") != null;
        }
    }
}
