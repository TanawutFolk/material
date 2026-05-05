using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PronesConnection.Models;
using PronesConnection.Property;
using System.Web.Mvc;

namespace PronesConnection.Interface
{
    public interface  IControllerAction<T>
    {
         JsonResult Search(T dataItem);
         JsonResult Insert(T dataItem);
         JsonResult Update(T dataItem);
         JsonResult Delete(T dataItem);
    }
}