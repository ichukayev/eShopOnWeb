﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessor.Models
{
    public class CatalogItemOrdered
    {
        public int CatalogItemId { get;  set; }
        public string ProductName { get;  set; }
        public string PictureUri { get;  set; }
    }
}
