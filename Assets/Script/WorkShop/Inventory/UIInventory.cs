using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] GameObject panelRoot;
    [SerializeField] Transform slotParent;
    [SerializeField] GameObject slotPrefab;

    private List<GameObject> pool = new();
    private Inventory inv;

    public void Bind(Inventory inventory)
    {
        if (inv != null) inv.Changed -= Refresh;
        inv = inventory;
        if (inv != null) inv.Changed += Refresh;
        Refresh();
    }

    public void Toggle()
    {
        bool show = !panelRoot.activeSelf;
        panelRoot.SetActive(show);
        if (show) Refresh();
    }

    void OnDestroy()
    {
        if (inv != null) inv.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (inv == null) return;
        EnsurePool(inv.Slots.Count);

        for (int i = 0; i < pool.Count; i++)
            pool[i].SetActive(i < inv.Slots.Count);

        for (int i = 0; i < inv.Slots.Count; i++)
        {
            var view = pool[i].GetComponent<SlotView>();
            view.Set(inv.Slots[i]);
        }
    }

    private void EnsurePool(int need)
    {
        while (pool.Count < need)
            pool.Add(GameObject.Instantiate(slotPrefab, slotParent));
    }
}