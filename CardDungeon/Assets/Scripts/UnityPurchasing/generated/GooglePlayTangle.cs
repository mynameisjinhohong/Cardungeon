// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZOvXy5FIMFxls2UY6j5JrLGfrUGHrAWPKt+btwNHOlhlYoWmIUPDjY+NBI79Ar/V8s7imTJnPwRPGYcSlxQaFSWXFB8XlxQUFZj8vBdkZTe/srd5PkLqCtnTNHsle6VESdEvoxNUAX/a4/LcpMw8pqgYydjyLTpZJZcUNyUYExw/k12T4hgUFBQQFRaRlqFGvP3nhBDfEQq0Uvkt0354zXsKz8ADiOeU2BE5dYcJXVgmRR8pOasJW7IP29SnYu+QF0AYiTOaRBrR7n+hhopysfhAUt/ATqXx+RUM8NSG6qgUftoZ5tshku+WB7paTXed/SeOyK4zZID50YUsjEvALu5UaKwDfh1hXy8ayNyazcve+F02zZR4NvW96pNZoXUPcBcWFBUU");
        private static int[] order = new int[] { 3,9,7,9,5,11,9,8,12,11,13,12,12,13,14 };
        private static int key = 21;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
