using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int Capacity { get; private set; }              // จำนวน "สล็อต"
    private readonly List<ItemStack> _slots;
    public event Action Changed;                           // ให้ UI subscribe

    public Inventory(int capacity)
    {
        Capacity = Mathf.Max(1, capacity);
        _slots = new List<ItemStack>(capacity);
    }

    public IReadOnlyList<ItemStack> Slots => _slots;

    /// เพิ่มไอเท็ม คืนค่าจำนวนที่ "เพิ่มได้จริง"
    public int Add(ItemDefinition def, int amount = 1)
    {
        if (def == null || amount <= 0) return 0;

        int remaining = amount;

        // เติมกองเดิมก่อน (copy -> modify -> assign back)
        for (int i = 0; i < _slots.Count && remaining > 0; i++)
        {
            var s = _slots[i];
            if (s.Def == def && !s.IsFull)
            {
                remaining = s.Add(remaining);
                _slots[i] = s; // assign back
            }
        }

        // ถ้ายังเหลือและมีสล็อตว่าง → สร้างกองใหม่
        while (remaining > 0 && _slots.Count < Capacity)
        {
            var s = new ItemStack { Def = def, Amount = 0 };
            remaining = s.Add(remaining);
            _slots.Add(s);
        }

        int added = amount - remaining;
        if (added > 0) Changed?.Invoke();
        return added;
    }

    /// ลบตามจำนวน (กระจายจากหลายกองได้)
    public bool Remove(ItemDefinition def, int amount = 1)
    {
        if (def == null || amount <= 0) return false;

        int need = amount;

        for (int i = _slots.Count - 1; i >= 0 && need > 0; i--)
        {
            var s = _slots[i];
            if (s.Def != def) continue;

            int take = Mathf.Min(need, s.Amount);
            s.Amount -= take;
            need -= take;

            if (s.Amount <= 0)
            {
                _slots.RemoveAt(i);
            }
            else
            {
                _slots[i] = s; // assign back
            }
        }

        bool ok = (need == 0);
        if (ok) Changed?.Invoke();
        return ok;
    }

    public int CountOf(ItemDefinition def)
    {
        int total = 0;
        foreach (var s in _slots)
            if (s.Def == def) total += s.Amount;
        return total;
    }
}