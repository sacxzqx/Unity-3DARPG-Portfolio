public enum InteractionType { NPC, Item, Chest, Portal }

public interface IInteractable
{
    InteractionType InteractionType { get; }
    void Interact();
}

