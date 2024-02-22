using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventListener - MonoBehaviour that listens to an VoidEvent and invokes a UnityEvent in response.
/// </summary>
public class EventListener : MonoBehaviour
{
	[SerializeField] private VoidEvent _event = default; // The VoidEvent to subscribe to.
	public UnityEvent listener; // The UnityEvent to invoke in response to the VoidEvent.

	/// <summary>
	/// Subscribe to the VoidEvent when this MonoBehaviour is enabled.
	/// </summary>
	private void OnEnable()
	{
		_event?.Subscribe(Respond);
	}

	/// <summary>
	/// Unsubscribe from the VoidEvent when this MonoBehaviour is disabled.
	/// </summary>
	private void OnDisable()
	{
		_event?.Unsubscribe(Respond);
	}

	/// <summary>
	/// Response method invoked when the subscribed VoidEvent is raised.
	/// Invokes the UnityEvent to trigger the associated Unity events.
	/// </summary>
	private void Respond()
	{
		listener?.Invoke();
	}
}
