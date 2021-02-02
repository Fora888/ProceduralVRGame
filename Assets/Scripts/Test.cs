using UnityEngine;

public class Test : MonoBehaviour
{
    ItemNameParser itemNameParser;
    // Start is called before the first frame update
    void Start()
    {
        itemNameParser = new ItemNameParser();
        Debug.Log(itemNameParser.ParseItemName("1,0,! (1)").Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
