# AI Schedules (schedules.lua) — Manual Function Index

> Auto-extraction failed (GMod syntax). Manually indexed.
> schedules.lua: 111 lines — schedule definitions + the `RunAI()` main tick.

## Schedule Definitions (called from SelectSchedule)

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:SCHEDULE_FACE(faceTask, customFunc)` | 14 | Create face schedule. Stationary NPCs skip if CanTurnWhileStationary=false | `BaseNPC.SetTurnTarget()` |
| `ENT:SCHEDULE_GOTO_POSITION(moveTask, customFunc)` | 23 | Move to last position. Aerial/aquatic bypass engine → AA_MoveTo | `BaseNPC.SelectSchedule()` dispatches |
| `ENT:SCHEDULE_GOTO_TARGET(moveTask, customFunc)` | 39 | Move to target entity. Aerial/aquatic bypass → AA_MoveTo | `BaseNPC.SelectSchedule()` |
| `ENT:SCHEDULE_COVER_ENEMY(moveTask, customFunc)` | 54 | Find cover from enemy. AA fallback to AA_IdleWander | `HumanNPC.FindCoverFrom()` |
| `ENT:SCHEDULE_COVER_ORIGIN(moveTask, customFunc)` | 74 | Find cover from origin | `HumanNPC` |
| `ENT:SCHEDULE_IDLE_WANDER()` | 102 | Wander schedule. AA → AA_IdleWander | `CreatureNPC.MaintainIdleBehavior()` |
| `ENT:SCHEDULE_IDLE_STAND()` | 107 | Idle stand. Checks IsMoving, NextIdleTime | `CreatureNPC` |

## Main AI Tick

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:RunAI()` | 162 | **THE main AI tick** (~100 lines). Called by engine every 0.1s. Checks freeze/barnacle, runs schedule, handles movement anims, turning/facing (ConstantlyFaceEnemy, stationary/attack facing, TurnData system) | `BaseNPC.OnUpdate()` → `UpdateSenses()` → `SelectSchedule()` |
| `ENT:OnTaskFailed(failCode, failString)` | 273 | Schedule failure handler: delay, ResetOnFail, RunCode_OnFail | `AIScheduleRunner` |
| `ENT:OnMovementFailed()` | 305 | Stub | — |
| `ENT:OnMovementComplete()` | 322 | Stub | — |
| `ENT:OnStateChange()` | 326 | Stub | — |
| `ENT:TranslateNavGoal(goalType, goalEnt)` | 330 | Override nav goal for GOALTYPE_ENEMY → ent:GetPos() | `BaseNPC` |

## Schedule Execution

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:StartSchedule(schedule)` | 349 | **Start a schedule** (~90 lines). Checks stationary/animation-only states, cleans previous, handles TurnData/IgnoreConditions | `AIScheduleRunner.Run()` |
| `ENT:DoSchedule(schedule)` | 440 | Execute current schedule tasks | `AIScheduleRunner` |
| `ENT:StopCurrentSchedule()` | 446 | Cleanly stop: clear goal, clear schedule, timers | `AIScheduleRunner.Cancel()` |
| `ENT:ScheduleFinished(schedule)` | 462 | Run OnFinish callback, clear conditions, reset turn data | `AIScheduleRunner` |
| `ENT:StartTask(task)` | 537 | Task lifecycle: start | `AIScheduleRunner` |
| `ENT:RunTask(task)` | 541 | Task lifecycle: run | `AIScheduleRunner` |
| `ENT:TaskTime()` | 545 | Get task timing | `AIScheduleRunner` |
| `ENT:IsScheduleFinished(schedule)` | 532 | Check if schedule done | `AIScheduleRunner` |

## Custom Tasks

| Lua Function | Line | What It Does | C# Target |
|---|---|---|---|
| `ENT:TASK_VJ_PLAY_ACTIVITY(taskStatus, data)` | 116 | Play activity via animation system | `AnimationDriver.PlayAnim()` + `AIScheduleRunner` |
| `ENT:TASK_VJ_PLAY_SEQUENCE(taskStatus, data)` | 142 | Play sequence via animation system | `AnimationDriver.PlaySequence()` |

---

**Total: 22 functions from schedules.lua (111 lines)**

Key insight: `RunAI()` is already ported as `BaseNPC.OnUpdate()` → `UpdateSenses()` → `SelectSchedule()`.
The schedule/task system is reimplemented as `AIScheduleRunner` (async/await instead of GMod timer-based).
