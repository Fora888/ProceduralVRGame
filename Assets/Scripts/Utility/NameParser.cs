using System;
using System.Collections;
using System.Collections.Generic;

public struct ItemNameParser
{
   public int[] ParseItemName(string name)
    {
        string[] temp;
        List<int> output = new List<int>();
        temp = name.Split(',');
        foreach(string entry in temp)
        {
            if(entry[0] != '!')
            {
                output.Add(int.Parse(entry));
            }
            else
            {
                return output.ToArray();
            }
        }
        return output.ToArray();
    }

}
