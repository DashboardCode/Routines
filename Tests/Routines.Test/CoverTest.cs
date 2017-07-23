using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DashboardCode.Routines.Test
{
    [TestClass]
    public class CoverTest
    {
        [TestMethod]
        public void IncludesCopyTest()
        {
            try
            {
                File.Open("",FileMode.Open); 
            }catch(ArgumentException ex)
            {
                ExceptionExtensions.Markdown(ex);
            }

            try
            {
                File.Open("notexist", FileMode.Open);
            }
            catch (FileNotFoundException ex) 
            {
                try
                {
                    var tmp = new FileLoadException("Test message", ex);
                    tmp.Data["my data"] = "my value";
                    tmp.HelpLink = "http://asdasd.adasd.asdasd/asdasd.html?asd=asd";
                    var tmp2 = new ApplicationException("asd", tmp);
                    throw tmp;
                }
                catch (Exception ex2)
                {
                    ExceptionExtensions.Markdown(ex2);
                }
            }

           
        }
    }
}
