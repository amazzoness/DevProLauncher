--光霊使いライナ
function c73318863.initial_effect(c)
	--flip
	local e1=Effect.CreateEffect(c)
	e1:SetDescription(aux.Stringid(73318863,0))
	e1:SetCategory(CATEGORY_CONTROL)
	e1:SetProperty(EFFECT_FLAG_CARD_TARGET)
	e1:SetType(EFFECT_TYPE_SINGLE+EFFECT_TYPE_FLIP)
	e1:SetTarget(c73318863.target)
	e1:SetOperation(c73318863.operation)
	c:RegisterEffect(e1)
end
function c73318863.filter(c)
	return c:IsFaceup() and c:IsAttribute(ATTRIBUTE_LIGHT) and c:IsControlerCanBeChanged()
end
function c73318863.target(e,tp,eg,ep,ev,re,r,rp,chk,chkc)
	if chkc then return chkc:IsLocation(LOCATION_MZONE) and chkc:IsControler(1-tp) and c73318863.filter(chkc) end
	if chk==0 then return true end
	if not e:GetHandler():IsStatus(STATUS_BATTLE_DESTROYED) then
		Duel.Hint(HINT_SELECTMSG,tp,HINTMSG_CONTROL)
		local g=Duel.SelectTarget(tp,c73318863.filter,tp,0,LOCATION_MZONE,1,1,nil)
		Duel.SetOperationInfo(0,CATEGORY_CONTROL,g,g:GetCount(),0,0)
	end
end
function c73318863.operation(e,tp,eg,ep,ev,re,r,rp)
	local c=e:GetHandler()
	local tc=Duel.GetFirstTarget()
	if c:IsRelateToEffect(e) and c:IsFaceup() and tc and tc:IsRelateToEffect(e) and c73318863.filter(tc) then
		c:CreateRelation(tc,RESET_EVENT+0x1fe0000)
		local e1=Effect.CreateEffect(c)
		e1:SetProperty(EFFECT_FLAG_CANNOT_DISABLE)
		e1:SetType(EFFECT_TYPE_SINGLE)
		e1:SetCode(EFFECT_SET_CONTROL)
		e1:SetValue(tp)
		e1:SetLabel(0)
		e1:SetReset(RESET_EVENT+0x1fc0000)
		e1:SetCondition(c73318863.ctcon)
		tc:RegisterEffect(e1)
		local e2=Effect.CreateEffect(c)
		e2:SetProperty(EFFECT_FLAG_CANNOT_DISABLE)
		e2:SetType(EFFECT_TYPE_SINGLE+EFFECT_TYPE_CONTINUOUS)
		e2:SetCode(EVENT_CHANGE_POS)
		e2:SetReset(RESET_EVENT+0x1fc0000)
		e2:SetOperation(c73318863.posop)
		e2:SetLabelObject(e1)
		tc:RegisterEffect(e2)
	end
end
function c73318863.ctcon(e)
	if e:GetLabel()==1 then return true end
	local c=e:GetOwner()
	local h=e:GetHandler()
	return h:IsAttribute(ATTRIBUTE_LIGHT) and not c:IsDisabled() and c:IsRelateToCard(h)
end
function c73318863.posop(e,tp,eg,ep,ev,re,r,rp)
	if e:GetLabelObject():GetLabel()~=0 then return end
	local c=e:GetOwner()
	local h=e:GetHandler()
	if h:IsAttribute(ATTRIBUTE_LIGHT) and not c:IsDisabled() and c:IsRelateToCard(h)
		and h:IsPreviousPosition(POS_FACEUP) and h:IsFacedown() then
		e:GetLabelObject():SetLabel(1)
	end
end
