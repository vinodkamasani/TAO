using System;
using System.Collections.Generic;
using System.Text;

namespace TAO.Application.HiringStrategies.Approve;

public sealed record ApproveHiringStrategyRequest(
    Guid ApprovedByUserId);