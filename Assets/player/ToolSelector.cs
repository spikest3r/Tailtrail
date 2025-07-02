using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

enum SelectedTool
{
    HOE = 0, WATERING = 1
}

public class ToolSelector : MonoBehaviour
{
    [SerializeField] PlayerFarming farmingModule;
    [SerializeField] PlayerWatering wateringModule;
    [SerializeField] ItemCounterController icc;
    [SerializeField] Sprite ICC_HoeIcon, ICC_WateringIcon, ICC_AxeIcon;

    const int ToolCount = 2;
    SelectedTool tool;
    bool shouldSwitch = false;
    bool triggerAction = false;

    struct ToolData
    {
        public int durability;
        public Sprite icon;
    }

    ToolData GetTool(int t = -1)
    {
        SelectedTool selected = t == -1 ? tool : (SelectedTool)t;
        ToolData result = new();
        switch(selected)
        {
            case SelectedTool.HOE:
                result.icon = ICC_HoeIcon;
                result.durability = StateSavingController.saveState.HoeDurability;
                break;
            case SelectedTool.WATERING:
                result.icon = ICC_WateringIcon;
                result.durability = StateSavingController.saveState.WateringDurability;
                break;
        }
        return result;
    }

    private void Awake()
    {
        icc.OnHide += onIccHide;
    }

    void onIccHide()
    {
        if (!triggerAction) return;
        shouldSwitch = false;
        triggerAction = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tool = SelectedTool.HOE;
        farmingModule.FarmingActive = true;
        wateringModule.WateringActive = false;
    }

    SelectedTool GetNextTool()
    {
        if ((int)tool + 1 < ToolCount)
        {
            return tool+1;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (!shouldSwitch)
            {
                // First press: preview
                ToolData data = GetTool();
                icc.ShowValue(data.icon, data.durability);

                shouldSwitch = true;
                triggerAction = true;
            }
            else
            {
                // Second press: confirm switch
                ToolData data = GetTool((int)GetNextTool());
                Debug.Log(data.durability);
                if (data.durability <= 0) return;

                tool = GetNextTool();
                icc.ShowValue(data.icon, data.durability);
                farmingModule.FarmingActive = tool == SelectedTool.HOE;
                wateringModule.WateringActive = tool == SelectedTool.WATERING;
            }
        }
    }
}
