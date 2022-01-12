using Godot;
using System;
using System.Collections.Generic;

public class DebugInstance : Label
{
    public class Stat {
        public Stat(string _statName, Godot.Object _objectRef, string _statRef, bool _isMethod) {
            statName = _statName;
            objectRef = _objectRef;
            statRef = _statRef;
            isMethod = _isMethod;
        }
        public string statName;
        public Godot.Object objectRef;
        public string statRef;
        public bool isMethod;
    }
    public class SystemInformation : Godot.Object {
        public SystemInformation(SceneTree _tree) {
            tree = _tree;
        }
        SceneTree tree;
        public float GetFramesPerSecond() {
            return Engine.GetFramesPerSecond();
        }
        public ulong GetStaticMemoryUsage() {
            return OS.GetStaticMemoryUsage();
        }
        public ulong GetDynamicMemoryUsage() {
            return OS.GetDynamicMemoryUsage();
        }
        public int GetAllInstances() {
            return tree.GetNodeCount();
        }
    }

    private List<Stat> stats = new List<Stat>();

    public void AddStat(string _statName, Godot.Object _objectRef, string _statRef, bool _isMethod) {
        stats.Add(new Stat(_statName, _objectRef, _statRef, _isMethod));
        //GD.Print(_statName, " exists: ",_objectRef != null && IsInstanceValid(_objectRef));
    }
    public void AddStat(string _statName, string _miscellaneous, string _statRef, bool _isMethod) { // Needs a separate function for engine information.
        switch (_miscellaneous)
        {
            case "system":
                SystemInformation systemInfo = new SystemInformation(GetTree());
                stats.Add(new Stat("Frames Per Second", systemInfo, "GetFramesPerSecond", true));
                stats.Add(new Stat("Number Of Instances", systemInfo, "GetAllInstances", true));
                stats.Add(new Stat("Static Memory Usage", systemInfo, "GetStaticMemoryUsage", true));
                stats.Add(new Stat("Dynamic Memory Usage", systemInfo, "GetDynamicMemoryUsage", true));
                break;
            default:
                GD.Print("Error when adding miscellaneous stat to debugger: ", _miscellaneous);
                break;
        }
    }
    public override void _Process(float delta)
    {
        string labelText = "";
        foreach (Stat stat in stats)
        {
            string value = null;
            if (stat.objectRef != null && WeakRef(stat.objectRef).GetRef() != null) {
                if (stat.isMethod) {
                    value = stat.objectRef.Call(stat.statRef)?.ToString();
                } else {
                    value = stat.objectRef.Get(stat.statRef)?.ToString();
                }
            }
            labelText += String.Format("{0}: {1}\n", stat.statName, value);
        }
        Text = labelText;
    }
}
