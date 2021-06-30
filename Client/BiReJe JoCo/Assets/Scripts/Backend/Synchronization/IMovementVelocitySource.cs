using UnityEngine;

namespace BiReJeJoCo.Backend
{
    public interface IMovementVelocitySource
    {
        Vector3 GetMovementVelocity();
    }
}
