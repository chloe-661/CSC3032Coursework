using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

public class StateMachine
{
   private IState currentState;
   private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type,List<Transition>>();
   private List<Transition> currentTransitions = new List<Transition>();
   private List<Transition> anyTransitions = new List<Transition>();
   private static List<Transition> emptyTransitions = new List<Transition>(0);

   public void Tick()
   {
      var transition = GetTransition();
      if (transition != null)
         SetState(transition.To);
      
      this.currentState?.Tick();
   }

   public void SetState(IState state)
   {
      if (state == this.currentState)
         return;
      
      this.currentState?.OnExit();
      this.currentState = state;
      
      this.transitions.TryGetValue(this.currentState.GetType(), out this.currentTransitions);
      if (this.currentTransitions == null)
         this.currentTransitions = emptyTransitions;
      
      this.currentState.OnEnter();
   }

   public void AddTransition(IState from, IState to, Func<bool> predicate)
   {
      if (this.transitions.TryGetValue(from.GetType(), out var transitions) == false)
      {
         transitions = new List<Transition>();
         this.transitions[from.GetType()] = transitions;
      }
      
      transitions.Add(new Transition(to, predicate));
   }

   public void AddAnyTransition(IState state, Func<bool> predicate)
   {
      this.anyTransitions.Add(new Transition(state, predicate));
   }

   private class Transition
   {
      public Func<bool> Condition {get; }
      public IState To { get; }

      public Transition(IState to, Func<bool> condition)
      {
         To = to;
         Condition = condition;
      }
   }

   private Transition GetTransition()
   {
      foreach(var transition in this.anyTransitions)
         if (transition.Condition())
            return transition;
      
      foreach (var transition in this.currentTransitions)
         if (transition.Condition())
            return transition;

      return null;
   }
}