using UnityEngine;

public class AudienceVoteUpdater : MonoBehaviour
{
    [SerializeField] private RadioButtonGroup radioButtonGroup;
    [SerializeField] private VoteGauge voteGauge;
    
    public RadioButtonGroup RadioButtonGroup => radioButtonGroup;
    public VoteGauge VoteGauge => voteGauge;
}
