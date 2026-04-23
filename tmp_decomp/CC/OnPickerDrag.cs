using System;
using UnityEngine;
using UnityEngine.Events;

namespace CC;

[Serializable]
public class OnPickerDrag : UnityEvent<Vector2>
{
}
