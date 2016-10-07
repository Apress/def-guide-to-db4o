using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Person
/// </summary>
public class Person
{

    private string _name;
    private int _age;

    public Person() { }

    public Person(String name, int age)
    {
        _name = name;
        _age = age;
    }


    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public int Age
    {
        get
        {
            return _age;
        }
        set
        {
            _age = value;
        }
    }


    public override string ToString()
    {
        return "[" + Name + ";" + Age + "]";
    }

}
