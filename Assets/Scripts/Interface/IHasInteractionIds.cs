using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGY
{
    public interface IHasInteractionIds
    {
        List<InteractionData.MsgId> InteractionIdList { get; }
    }
}
