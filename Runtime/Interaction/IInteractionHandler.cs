using UnityEngine;

namespace DuksGames.Argon.Interaction
{
    public enum InteractionStage { Beginning, End } 
    public enum InteractionSucess { Success, Fail }
    
    public class InteractonStatus
    {
        public InteractionStage Stage = InteractionStage.Beginning;
        public InteractionSucess Sucess = InteractionSucess.Success;
    }

    public class InteractionHandlerInfo
    {
        public GameObject Source; // e.g. the player
        public Component InteractionTarget; // e.g. a ClickInteractionHandler
        public InteractonStatus InteractionStatus = new();
    }

    public interface IClickInteractionHandler
    {
        bool CouldInteract(InteractionHandlerInfo handlerInfo);
        void Interact(InteractionHandlerInfo handlerInfo);
    }

    // public interface IInteractionEventResponder
    // {
    //     void Interact(InteractionHandlerInfo handlerInfo);
    // }


    public interface IClickFeedback
    {
        void GiveFeedback();
    }



    public interface ITriggerEnterHandler
    {
        void HandleTriggerEnter(InteractionHandlerInfo handlerInfo);
    }

    public interface ITriggerExitHandler
    {
        void HandleTriggerExit(InteractionHandlerInfo handlerInfo);
    }
}