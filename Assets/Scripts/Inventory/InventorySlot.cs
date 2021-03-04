public class InventorySlot
{
    public float count;
    public string name;

    public float RetriveItem(float count)
    {
        float retrivedCount = 0;
        if (this.count != 0)
        {
            if (this.count <= count)
            {
                retrivedCount = this.count;
                this.count = 0;
                this.name = null;
            }
            else
            {
                retrivedCount = count;
                this.count -= count;
            }
        }
        return retrivedCount;
    }
}
