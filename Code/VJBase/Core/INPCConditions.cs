namespace VJBase;

public interface INPCConditions
{
    void SetCondition(Condition cond);
    void ClearCondition(Condition cond);
    bool HasCondition(Condition cond);
    void SetIgnoreConditions(List<Condition> conds);
    void RemoveIgnoreConditions(List<Condition> conds);
}
