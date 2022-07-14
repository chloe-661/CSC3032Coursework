# CSC3032Coursework
Newcastle University - 2021/22 - 3rd Year

	MAJOR PROJECT IN COMPUTER SCIENCE MODULE

	Final year project and dissertation

	Poster Submitted: 25/03/2022
	Dissertation Submitted: 13/05/2022
	Demonstration Submitted: 16/05/2022
	
Marks and Feedback

	OVERALL
	
	Marks: 63
		
.	

	POSTER
	
	Marks: ??
	Feedback: 
		... 
.

	DEMONSTRATION
	
	Marks: ??
	Feedback: 
		...

.

	DISSERTATION 
	
	Marks: ??
	Feedback:
		- Supervisor (Dr G Bergami)
			
			Introduction:
			The student provides an introduction which is sometimes incoherent: in particular, the structure of the dissertation should be the last thing in the introduction after describing the other points. Aims and Objectives were separated from the introduction with a separate section, while they should have been in §1. The dissertation contains some motivation and sketched aims and objectives. Still, the student should consider that "explore and research" or "Develop" are not actual objectives that validate the pursuance of the aim, rather than the methodology required for implementing the objectives. In particular, the objective is a research question determine which are the most relevant features that make the algorithm preferable to another solution, evaluate the algorithms analysed in the dissertation, and list the outcome of such findings.
			Still, Objectives #6, #7, and #8 are evidence of such a thought process. Still, the student should have elaborated more on the objectives, thus listing the things being done to achieve those and the results obtained so far. The introduction is good, and no significant errors and minor omissions, mainly due to the lack of in-depth knowledge of the subject. Still, some assumptions are incorrect: despite DFAs (usually referred to as FSMs in the dissertation) being finite, they might describe an infinite language, as these machines usually contain loops leading to potentially infinite traces. Also, in the usual video games implementation, each state is implemented as a single object maintaining stateful information, thus allowing one to remember previous decisions if required.
			Furthermore, the student claims that the AI of choice (RL) can remember past decisions and use that to decide on the best action to perform upon environment perception. Still, RL heavily relies on stateful information as DFA; thus, states are how RL and DFA share to remember previous decisions. If the student wanted to investigate proper memory mechanisms, she should have investigated LSTM or inference mechanisms. Still, any unsupervised approach (replanning) will outperform any training-based AI, for which it

			Background Review:
			The student provided some background. Still, the student limits the understanding of the RL approach to the Unity documentation, which does not provide enough evidence for better explanation and knowledge of how the mechanism works. E.g., some papers on PPO should have been referenced instead. Despite the student referring to the state machine implemented as FSM, the graph later on implemented in the dissertation seems more like a Markov Chain (similar to the MDP seen in CSC3232), as the transition labels are also associated with probabilistic values. In addition to this, the graphs are also nested, thus implying that the graph of choice is a probabilistic statechart that can be still represented as an un-nested graph. Still, the way to describe the FSMs in the implementation section is not accurate, as some of the points should have been represented to comply with the literature.
			A good literature reference would have disambiguated the difference between a DFA (referred to as FSM) and an MC and provided a correct graphical representation of the FSMs.
			Furthermore, a better understanding of this other part of literature would have helped the student draw parallelisms and analyse the pros and cons of the two competing approaches.
			Conclusions are limited to the other projects used as a blueprint for the dissertation, and those are the only evidence guiding the student. Please observe that usually, you need to study the literature of reference first. Then you can make informed choices to know what to alter/change in already-existing implementations. Therefore, the conclusions are relatively weak, but these game references provide some additional project context relevant for better understating the use case scenario of interest. Thus, the literature review is not exploited as it should, i.e., comparing different approaches, implementations, and formal methods exploited by these implementations. As a consequence, no independent conclusion was noticed. More complex and diverse sources providing some explanations on the techniques of interest should have been considered.

			What was done and how:
			Chloe provides a significant amount of implementation details. The methods being used are appropriate in as much as the dissertation explains the implementation of the objects relevant for the dissertation's final evaluation section. The student shows the ability to implement nested MC, where the way probability values are inferred from random number generators are discussed (please observe that C# already provides RNG returning uniform distributions between 0 and 1: NextDouble!)The student also managed to set up an ML library and showcased that while discussing the way that was connected with Unity. The process still might have been better defined if, instead of relying on a fixed training configuration, she should have first trained the model by changing the hyper-parameters to decide on the best configuration for solving the task. This is a tedious but required process in ML, as not all of the parameters will work for all possible problems! This shows that parts of the process were not fully thought through. The project does not deliver novel solutions rather than exploiting the existing ones. Still, the implementation was tailored to the use case scenario. The solution did not provide innovative results, but there is very little evidence on understanding the strength and limitations of AI vs nested MC. Still, some empirical understanding was possible via some preliminary experiments
			
			Results and Evaluation:
			The evaluation is relatively weak: some sentences are not very well laid out ("it can be seen from the two graphs that the State AI takes an aggressive behaviour": why can I infer that from the data?)Still, the student encountered some problems due to the limitations of the ML library not supporting efficient memory management systems, as they tend to go OOM easily. Nevertheless, I do not think that enough training time would have led to better results: as a matter of fact, the student did not test other possible perceptions for the agent and, usually, it is always better to 1) make a clear correlation to the signals/rewards and the game goals and 2) send as detailed data as possible. The student should have better investigated both the hyper-parameter and the agents' perceptions. Still, that might have been challenging, as the ML toolkit provided by Unity crashes most of the time! The student was also able to assess which critical issues might have been, thus suggesting that different perception settings might have been used. Still, the student showed good programming skills, as the MC approach was, in the very end, the most effective. Although the dissertation might have been rewritten by claiming that unsupervised approaches will consistently outperform training-based ones, this cannot be claimed, as there was no insurance that both the training hyper-parameters and the set of agent's perceptions are the best possible. In particular, this is suggested by some of the student's claims (e.g., the agent was trying to get to the "hardpoint", but it had no idea where it was).

			Conclusions:
			Despite the difficulties encountered, the dissertation contains some results and evaluations. The evidence cannot be substantiated, as additional training settings are required. Still, the student concluded that one implementation clearly outperformed the other. Due to the lack of experiments, the results are only partially convincing. The future works are limited to the extension with further training. Some ideas are hinted at and not substantiated (e.g., how to make AI nearer to human behaviour or mix AI with human players). More importantly, any Future Work should be substantiated with the further goals to be achieved. Nevertheless, the thesis offers no novel proposal nor elaborates on how to extend the current work significantly besides re-doing the experiments. If RL is not deemed adequate for small groups, no alternatives are proposed to mitigate the effects.
			
			Form/References:
			The dissertation exhibits a coherent structure. As per previous mentions, the dissertation still has content omissions, especially on state of the art. Still, it is well structured, has no noticeable language defect, and is easy to read. The presentation is adequate, figures are essential, and listings for the ML parts add some value, as this might be the only way to assess a proprietary product with no FOSS API. The dissertation is well presented, albeit the introduction and related works are gaunt. The dissertation is adequately structured, and there are some spelling mistakes ("they produced some interested behaviours" vs "interesting" in §5.4). Against all odds, the writing is concise and clear. Although the student attempted to meet technical standards (e.g., computer specifications, hyper-parameters), a more in-depth discussion of the related works would have clearly made an outstandingly written dissertation.
			
		- Second Marker (Tong Xin)
			
			Introduction:
			This project aims to investigate the effectiveness of DL techniques against State AI in hardpoint games. The Abstract section is empty. Expect more introduction to the different AI approaches. Why deep reinforcement learning? What is State AI? It is worth thinking that there may have an “overfit” problem if given an “unlimited amount of time to train” the ML models.

			Background Review:
			The background section is able to cover necessary knowledge and most relevant works. It would more beneficial to introduce ML before the bias in ML. Expect more evidence-based discussion as it can help to strengthen the clarification.

			What was done and how:
			This section further described the design and implementation of the experiments with a good level of detail. A variety of visual aids are employed to support the demonstration, including flow charts, pictures, tables, etc. The constructions are easy to follow and replicate. Some of the scales of the tensorboard are inconsistent which makes it difficult to compare.
			
			Results and Evaluation:
			It is evident that a substantial amount of experiments have been done. Expect more quantitative evaluation on the training of the AI model. It is unclear with statements such as “…to a good standard and test…”. Instead, a visualisation for the model convergence should be provided. How the “data” is distributed for the training and testing? What are the performance matrices for the AI model, such as accuracy, precision, or recall?

			Conclusions:
			The conclusion section can comprehensively summarise the project and its limitation which lead to reasonable future work. It would benefit the audience to recall the objectives by spelling them out or summarising the points that this project is supposed to look into.

			Form/References:
			Some format errors in the table of content. The dissertation structure section is a bit over detailed and it usually follows after the introduction section. Consider using more diverse references. Typically conference or journal papers are more convincing than blogs or wiki.
