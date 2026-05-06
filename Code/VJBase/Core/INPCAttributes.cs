using Sandbox;

namespace VJBase;

public interface INPCAttributes
{
    int GetNPCState();
    void SetNPCState(int state);
    int GetNavType();

    GameObject GetEnemy();
    void SetEnemy(GameObject enemy);

    void SetLastPosition(Vector3 pos);
    Vector3 GetLastPosition();
    float GetMaxYawSpeed();
    void SetMaxYawSpeed(float speed);

    bool IsMoving();
    bool IsBusy(string context);

    void StopMoving();
    void ClearGoal();
    void ClearSchedule();

    float GetIdealYaw();
    bool IsFacingIdealYaw();
    void SetIdealYawAndUpdate(float yaw);
    float GetMoveDelay();

    int Disposition(GameObject other);
    void AddEntityRelationship(GameObject other, int disposition, int priority);
}
