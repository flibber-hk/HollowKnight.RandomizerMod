﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HutongGames.PlayMaker;

namespace RandomizerMod.Extensions
{
    internal static class PlayMakerFSMExtensions
    {
        private static FieldInfo fsmStringParams = typeof(ActionData).GetField("fsmStringParams", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playMakerFSMfsm = typeof(PlayMakerFSM).GetField("fsm", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo fsmStates = typeof(Fsm).GetField("states", BindingFlags.NonPublic | BindingFlags.Instance);

        public static List<FsmString> GetStringParams(this ActionData self)
        {
            return (List<FsmString>)fsmStringParams.GetValue(self);
        }

        public static void AddState(this PlayMakerFSM self, FsmState state)
        {
            Fsm fsm = (Fsm)playMakerFSMfsm.GetValue(self);
            List<FsmState> states = ((FsmState[])fsmStates.GetValue(fsm)).ToList();
            states.Add(state);
            fsmStates.SetValue(fsm, states.ToArray());
        }

        public static FsmState GetState(this PlayMakerFSM self, string name)
        {
            foreach (FsmState state in self.FsmStates)
            {
                if (state.Name == name)
                {
                    return state;
                }
            }

            return null;
        }

        public static void RemoveActionsOfType<T>(this FsmState self) where T : FsmStateAction
        {
            List<FsmStateAction> actions = new List<FsmStateAction>();

            foreach (FsmStateAction action in self.Actions)
            {
                if (!(action is T))
                {
                    actions.Add(action);
                }
            }

            self.Actions = actions.ToArray();
        }

        public static T[] GetActionsOfType<T>(this FsmState self) where T : FsmStateAction
        {
            List<T> actions = new List<T>();

            foreach (FsmStateAction action in self.Actions)
            {
                if (action is T)
                {
                    actions.Add((T)action);
                }
            }

            return actions.ToArray();
        }

        public static void ClearTransitions(this FsmState self)
        {
            self.Transitions = new FsmTransition[0];
        }

        public static void RemoveTransitionsTo(this FsmState self, string toState)
        {
            List<FsmTransition> transitions = new List<FsmTransition>();

            foreach (FsmTransition transition in self.Transitions)
            {
                if (transition.ToState != toState)
                {
                    transitions.Add(transition);
                }
            }

            self.Transitions = transitions.ToArray();
        }

        public static void AddTransition(this FsmState self, string eventName, string toState)
        {
            List<FsmTransition> transitions = self.Transitions.ToList();

            FsmTransition trans = new FsmTransition();
            trans.ToState = toState;

            if (FsmEvent.EventListContains(eventName))
            {
                trans.FsmEvent = FsmEvent.GetFsmEvent(eventName);
            }
            else
            {
                trans.FsmEvent = new FsmEvent(eventName);
            }

            transitions.Add(trans);

            self.Transitions = transitions.ToArray();
        }

        public static void AddFirstAction(this FsmState self, FsmStateAction action)
        {
            List<FsmStateAction> actions = new List<FsmStateAction>();
            actions.Add(action);
            actions.AddRange(self.Actions);
            self.Actions = actions.ToArray();
        }

        public static void AddAction(this FsmState self, FsmStateAction action)
        {
            List<FsmStateAction> actions = self.Actions.ToList();
            actions.Add(action);
            self.Actions = actions.ToArray();
        }
    }
}
