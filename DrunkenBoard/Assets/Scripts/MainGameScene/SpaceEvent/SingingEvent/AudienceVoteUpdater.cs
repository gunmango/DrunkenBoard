using UnityEngine;
using UnityEngine.UI;

public class AudienceVoteUpdater : MonoBehaviour
{
    [SerializeField] private RadioButtonGroup radioButtonGroup;
    [SerializeField] private VoteGauge voteGauge;
    [SerializeField] private Button startCountDownButton;
    [SerializeField] private NetworkTimer countDownTimer;
    
    public RadioButtonGroup RadioButtonGroup => radioButtonGroup;
    public VoteGauge VoteGauge => voteGauge;
    public Button StartCountDownButton => startCountDownButton;
    public NetworkTimer CountDownTimer => countDownTimer;
}
