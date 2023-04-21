using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MatteoBenaissaLibrary.SpriteView
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteView : MonoBehaviour
    {
    
        #region Serialize Fields

        [Header("States"), SerializeField] 
        private string _defaultStateName;

        [SerializeField] 
        private List<Animation> _statesList;

        [Header("Action"), SerializeField] 
        private List<Animation> _actionsList;

        [HideInInspector] public UnityEvent OnActionEnd = new UnityEvent();
    
        #endregion

        #region Private values

        //sprite
        private SpriteRenderer _spriteRenderer;
        private float _changeCountdown;
        private int _currentSprite;
        //state
        [HideInInspector] public Animation Animation;
        private int _currentStateIndex;
        //actions
        private bool _isPlayingAction;
        //dictionaries
        private Dictionary<string, Animation> _stateDictionary = new Dictionary<string, Animation>();
        private Dictionary<string, Animation> _actionDictionary = new Dictionary<string, Animation>();

        #endregion
    
    
        private void Awake()
        {
            //state
            Animation = _statesList.First(x => x.Name == _defaultStateName);
            _currentStateIndex = _statesList.IndexOf(Animation);

            //sprite
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _changeCountdown = Animation.TimeBetweenFrames;
            ResetAnimation();
        
            //dictionary
            foreach (Animation state in _statesList)
            {
                _stateDictionary.Add(state.Name, state);
            }
            foreach (Animation action in _actionsList)
            {
                _actionDictionary.Add(action.Name, action);
            }
        }

        private void Update()
        {
            Animate();
        }

        #region Animation methods

        private void Animate()
        {
            _changeCountdown -= Time.deltaTime;

            if (_changeCountdown <= 0)
            {
                _changeCountdown = Animation.TimeBetweenFrames;

                _currentSprite++;
                if (_currentSprite >= Animation.SpriteSheet.Count)
                {
                    _currentSprite = 0;
                
                    //if action -> reset to current state
                    if (_isPlayingAction)
                    {
                        _isPlayingAction = false;
                        OnActionEnd.Invoke();
                        Animation = _statesList[_currentStateIndex];
                        _spriteRenderer.sprite = Animation.SpriteSheet[_currentSprite];
                    }
                }

                if (Animation.SpriteSheet.Count > 0)
                {
                    _spriteRenderer.sprite = Animation.SpriteSheet[_currentSprite];
                }
            }
        }
    
        private void ResetAnimation()
        {
            if (Animation.SpriteSheet.Count > 0)
            {
                _spriteRenderer.sprite = Animation.SpriteSheet[0];
            }
            _currentSprite = 0;
        }

        #endregion

        #region Play State/Action public methods

        public void PlayState(string state)
        {
            if (Animation.Name == state || _isPlayingAction)
            {
                return;
            }

            Animation = _stateDictionary[state];
            _currentStateIndex = _stateDictionary.Values.ToList().IndexOf(Animation);
            ResetAnimation();
        }

        public void PlayAction(string state)
        {
            _isPlayingAction = true;
            Animation = _actionDictionary[state];
            ResetAnimation();
        }

        #endregion
    
    
    }
}
