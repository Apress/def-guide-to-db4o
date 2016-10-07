using System;
using com.db4o;

namespace com.db4o.dg2db4o.chapter12
{
    class LoginManager
    {
        string _host;
        int _port;
        static int MAXIMUM_USERS = 4;

        public LoginManager(String host, int port)
        {
            _host = host;
            _port = port;
        }

        public ObjectContainer Login(String username, String password)
        {
            ObjectContainer objectContainer;
            try
            {
                objectContainer = Db4o.OpenClient(_host, _port, username, password);
            }
            catch (System.IO.IOException e)
            {
                return null;
            }

            bool allowedToLogin = false;

            for (int i = 0; i < MAXIMUM_USERS; i++)
            {
                String semaphore = "login_limit_" + (i + 1);
                if (objectContainer.Ext().SetSemaphore(semaphore, 0))
                {
                    allowedToLogin = true;
                    Console.WriteLine("Logged in as " + username);
                    Console.WriteLine("Acquired semaphore " + semaphore);
                    break;
                }
            }

            if (!allowedToLogin)
            {
                Console.WriteLine("Login not allowed for " + username + ": max clients exceeded");
                objectContainer.Close();
                return null;
            }

            return objectContainer;
        }

    }
}
