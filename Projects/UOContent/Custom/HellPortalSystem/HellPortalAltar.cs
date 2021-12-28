using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.HellPortalSystem;

public class HellPortalAltar : Item
{
    private Dictionary<HellPortalState, StateProperty> _stateProperties;
    private Dictionary<StateTransition, HellPortalState> _transitions;
    private HellPortalState _currentState;
    private int _currentTokenLoad;
    private int _maxTokensInState;

    //===========================================
    private Timer _tokenDecayTimer;
    private int _tokenDecayTimerSeconds;
    private event Action<int, int> _currentTokenLoadChange;
    //===========================================


    [CommandProperty(AccessLevel.GameMaster)]
    public int MaxTokensInState
    {
        get => _maxTokensInState;
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public int CurrentTokenLoad
    {
        get => _currentTokenLoad;
        set
        {
            _currentTokenLoadChange?.Invoke(_currentTokenLoad, value);
            _currentTokenLoad = value;
        }
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public HellPortalState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    [CommandProperty(AccessLevel.GameMaster)]
    public int TokenDecayTimerSeconds
    {
        get => _tokenDecayTimerSeconds;
        set => _tokenDecayTimerSeconds = value;
    }


    [Constructible]
    public HellPortalAltar() : base(13902)
    {
        Movable = false;
        Hue = 0;
        _currentTokenLoad = 0;
        _maxTokensInState = 20;
        _tokenDecayTimerSeconds = 10;
        Name = "Sleeping Altar";

        // Initialize action handler on token load change
        _currentTokenLoadChange += OnCurrentTokenLoadChange;

        // Initialize state properties (ItemID, Name per stage etc.)
        _stateProperties = new Dictionary<HellPortalState, StateProperty>
        {
            // Register property levels
            { HellPortalState.Inactive, new StateProperty() { Name = "Sleeping Altar", ItemID = 13902, Hue = 0, MaxTokensInState = 20 } },
            { HellPortalState.One, new StateProperty() { Name = "Awaken Altar", ItemID = 13902, Hue = 1161, MaxTokensInState = 30 } },
            { HellPortalState.Two, new StateProperty() { Name = "Fredgling Altar", ItemID = 13903, Hue = 0, MaxTokensInState = 40 } },
            { HellPortalState.Three, new StateProperty() { Name = "Developing Altar", ItemID = 13903, Hue = 1161, MaxTokensInState = 50 } },
            { HellPortalState.Four, new StateProperty() { Name = "Mature Altar", ItemID = 13904, Hue = 0, MaxTokensInState = 60 } },
            { HellPortalState.Five, new StateProperty() { Name = "Altar of Fury", ItemID = 13904, Hue = 1161, MaxTokensInState = 70 } }

        };

        // Initialize state machine
        _currentState = HellPortalState.Inactive;
        _transitions = new Dictionary<StateTransition, HellPortalState>
        {
            // Register states
            //=============
            // Grow states
            //=============
            { new StateTransition(HellPortalState.Inactive, HellPortalCommand.Grow), HellPortalState.One },
            { new StateTransition(HellPortalState.One, HellPortalCommand.Grow), HellPortalState.Two },
            { new StateTransition(HellPortalState.Two, HellPortalCommand.Grow), HellPortalState.Three },
            { new StateTransition(HellPortalState.Three, HellPortalCommand.Grow), HellPortalState.Four },
            { new StateTransition(HellPortalState.Four, HellPortalCommand.Grow), HellPortalState.Five },
            //=============
            // Shrink states
            //=============
            { new StateTransition(HellPortalState.One, HellPortalCommand.Shrink), HellPortalState.Inactive },
            { new StateTransition(HellPortalState.Two, HellPortalCommand.Shrink), HellPortalState.One },
            { new StateTransition(HellPortalState.Three, HellPortalCommand.Shrink), HellPortalState.Two },
            { new StateTransition(HellPortalState.Four, HellPortalCommand.Shrink), HellPortalState.Three },
            { new StateTransition(HellPortalState.Five, HellPortalCommand.Shrink), HellPortalState.Four }
        };
    }


    public HellPortalAltar(Serial serial) : base(serial)
    {
    }

    public void OnCurrentTokenLoadChange(int amountBefore, int amountAfter)
    {
        // Ignore if the current amount is 0
        //=======================================
        if (amountAfter == 0)
        {
            return;
        }

        // Start the decay timer on adding tokens
        // Shrink the state if delayed
        //=======================================
        if (amountAfter > amountBefore)
        {
            if (_tokenDecayTimer != null)
                _tokenDecayTimer.Stop();

            _tokenDecayTimer = Timer.DelayCall(
                TimeSpan.FromSeconds(_tokenDecayTimerSeconds),
                TimeSpan.FromSeconds(_tokenDecayTimerSeconds),
                delegate()
                {
                    _currentTokenLoad = 0;
                    GenerateFlameWave(this);
                    MoveNextHellPortalState(HellPortalCommand.Shrink);
                }
            );
        }

        // Grow of not stage five
        //=======================================
        if (amountAfter == _maxTokensInState && _currentState != HellPortalState.Five)
        {
            _currentTokenLoad = 0;
            MoveNextHellPortalState(HellPortalCommand.Grow);
        }
    }

    public void UpdateAltarProperties(StateProperty property)
    {
        Name = property.Name;
        ItemID = property.ItemID;
        Hue = property.Hue;
        _maxTokensInState = property.MaxTokensInState;
    }

    public HellPortalState GetNextHellPortalState(HellPortalCommand command)
    {
        StateTransition transition = new StateTransition(_currentState, command);
        if (!_transitions.TryGetValue(transition, out HellPortalState nextState))
            return _currentState;

        return nextState;
    }

    public HellPortalState MoveNextHellPortalState(HellPortalCommand command)
    {
        _currentState = GetNextHellPortalState(command);
        if (!_stateProperties.TryGetValue(_currentState, out StateProperty property))
            throw new Exception("No value found");
        UpdateAltarProperties(property);
        return _currentState;
    }

    public override void OnSingleClick(Mobile from)
    {
        if (Deleted || !from.CanSee(this))
            return;
        LabelTo(from, Name);
        LabelTo(from, $"Stage: {Enum.GetName(_currentState)}", 1161);
        LabelTo(from, $"{_currentTokenLoad}/{_maxTokensInState}", 1161);

    }

    public override void OnDoubleClick(Mobile from)
    {
        if (from.InRange(this.GetWorldLocation(), 1))
        {

            this.SendLocalizedMessageTo(from, 1010086);
            from.Target = new HellPortalTarget(this);

        }
    }

    public void GenerateFlameWave(Item itm)
    {
        List<Mobile> list = new List<Mobile>();

        foreach (Mobile m in this.GetMobilesInRange(8))
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
                list.Add(m);
            else if (m.Player && m.Alive)
                list.Add(m);
        }

        foreach (Mobile m in list)
        {
            m.SendMessage("The altar seems to be becoming unstable!");
            m.SendMessage("*Vas Grav Consume !*");
        }

        new FlameWave(itm).Start();
    }

    public override void Serialize(IGenericWriter writer)
    {
        base.Serialize(writer);

        writer.Write(0); // version
        writer.Write((int)_currentState);
        writer.Write(_currentTokenLoad);
        writer.Write(_maxTokensInState);
        writer.Write(_tokenDecayTimerSeconds);
    }

    public override void Deserialize(IGenericReader reader)
    {
        base.Deserialize(reader);

        var version = reader.ReadInt();
        _currentState = (HellPortalState)reader.ReadInt();
        _currentTokenLoad = reader.ReadInt();
        _maxTokensInState = reader.ReadInt();
        _tokenDecayTimerSeconds = reader.ReadInt();
    }
}
