using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;

namespace ProxyChekers
{ //https://free-proxy-list.net/
    internal class Program
    {
        
        /// <summary>
        /// статистическое поле сохраняющее дальше в коде последнее значение.
        /// </summary>
        private static int keepValues = 0;


        /// <param Cheker = "Cheker" > Метод для чека прокси. min & max диапозон для проксей</param >
        public static void Cheker(string[] ip, int min, int max)
        {
            /// оборочивание в цикл for для отлова исключение и перехождения к next proxy
            for (var j = min; j <= max; j++)
            {
                /// <remarks>
                /// переменная для блока catch. потому что i цикла for из блока try недоступна
                /// </remarks > 
                var empty = 0;
                try
                {
                    for (var i = min; i <= max; i++)
                    {
                        /// чек прокси
                        empty = i;
                        var proxy = new WebProxy(ip[i], true);
                        var webRequest = WebRequest.CreateHttp("https://www.google.com/");
                        webRequest.Proxy = proxy;
                        webRequest.Timeout = 2000;
                        var response = (HttpWebResponse)webRequest.GetResponse();
                        var respString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        Console.WriteLine(ip[empty] + "живой");
                    }

                }
                /// исключения
                catch (WebException)
                {
                    Console.WriteLine(ip[empty] + "мертвый");

                }
            }
        }

        private static void Main()
        {
            var carryover = new List<string>();

            int min, max;
            /// <remarks>
            /// кол-во прокси на поток
            /// </remarks> 
            int countProxyForOneThread;
            /// <remarks>
            /// кол-во потоков
            /// </remarks> 
            var threads = 7;
            /// <remarks>
            /// для цикла foreach
            /// </remarks> 
            var chekadd = false;
            string writetoprox = null;
            /// <remarks>
            /// поднятия прокси апишников из файла
            /// </remarks> 
            var file = File.ReadAllText(@"E:\projects\proxy.txt");

            /// считывание прокси без пробелов и тд в листе
            foreach (var symbols in file)
                if ((symbols != ' ') && (symbols != '\r') && (symbols != '\n'))
                {
                    writetoprox += symbols;
                    chekadd = false;
                }
                else
                {
                    if (chekadd == false) carryover.Add(writetoprox);
                    chekadd = true;
                    writetoprox = null;
                }
            /// <remarks>
            /// Делаю переменную ссылки на массив типа string, а затем присваиваю ей коллекцию, которую с помощью метода ToArray я перевёл в массив.
            /// </remarks >  
            var _link = carryover.ToArray();
            countProxyForOneThread = _link.Length / threads;

            /// <remarks>
            /// Массив потоков
            /// </remarks > 
            var threads1 = new Thread[7];

            /// /// <remarks>
            /// Присвоение значение переменным 
            /// </remarks > 
            min = 0;
            max = 0;
            int range;
            /// в этом цикле for я добавляю в массив потоков лямбда-выражение, которое вмещает в себя метод Checker
            for (var i = 1; i <= threads; i++)
            {
                /// конечное значение для проверки
                range = countProxyForOneThread * i;
                /// диапозон проверки прокси от 0 и до countProxyForOneThread
                if (i == 0)
                {
                    threads1[i - 1] = new Thread(() => Cheker(_link, 0, countProxyForOneThread));
                    /// запоминаем индекс проверенного прокси
                    keepValues = countProxyForOneThread;
                }
                else
                {
                    /// добавляем поток, где от значений переменной _link до значения переменной mt ( mt отображет тот диапазон, до которого метод должен проверить ) метод проверяет прокси.
                    threads1[i - 1] = new Thread(() => Cheker(_link, keepValues, range));
                    /// запоминаем значение которое было конечным. до которого мы проверяли
                    keepValues = range;
                }
            }
            /// запуск потоков в цикле
            for (var i = 0; i < threads1.Length; i++) threads1[i].Start();
        }
        
    }
}
            
        
        

    
