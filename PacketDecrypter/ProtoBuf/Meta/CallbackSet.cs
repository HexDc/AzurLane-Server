using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
    // Token: 0x020003D8 RID: 984
    public class CallbackSet
	{
		// Token: 0x060032F2 RID: 13042 RVA: 0x0012841E File Offset: 0x0012681E
		internal CallbackSet(MetaType metaType)
		{
			if (metaType == null)
            {
                throw new ArgumentNullException("metaType");
            }

            this.metaType = metaType;
		}

		// Token: 0x170002BB RID: 699
		internal MethodInfo this[TypeModel.CallbackType callbackType]
		{
			get
			{
				switch (callbackType)
				{
					case TypeModel.CallbackType.BeforeSerialize: return beforeSerialize;
					case TypeModel.CallbackType.AfterSerialize: return afterSerialize;
					case TypeModel.CallbackType.BeforeDeserialize: return beforeDeserialize;
					case TypeModel.CallbackType.AfterDeserialize: return afterDeserialize;
					default: throw new ArgumentException("Callback type not supported: " + callbackType.ToString(), "callbackType");
				}
			}
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x001284A8 File Offset: 0x001268A8
		internal static bool CheckCallbackParameters(TypeModel model, MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType != model.MapType(typeof(SerializationContext)))
				{
					if (parameterType != model.MapType(typeof(Type)))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x00128514 File Offset: 0x00126914
		private MethodInfo SanityCheckCallback(TypeModel model, MethodInfo callback)
		{
			metaType.ThrowIfFrozen();
			if (callback == null)
			{
				return callback;
			}
			if (callback.IsStatic)
			{
				throw new ArgumentException("Callbacks cannot be static", "callback");
			}
			if (callback.ReturnType != model.MapType(typeof(void)) || !CallbackSet.CheckCallbackParameters(model, callback))
			{
				throw CallbackSet.CreateInvalidCallbackSignature(callback);
			}
			return callback;
		}

		// Token: 0x060032F6 RID: 13046 RVA: 0x0012857E File Offset: 0x0012697E
		internal static Exception CreateInvalidCallbackSignature(MethodInfo method)
		{
			return new NotSupportedException("Invalid callback signature in " + method.DeclaringType.FullName + "." + method.Name);
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x060032F7 RID: 13047 RVA: 0x001285A5 File Offset: 0x001269A5
		// (set) Token: 0x060032F8 RID: 13048 RVA: 0x001285AD File Offset: 0x001269AD
		public MethodInfo BeforeSerialize
        {
            get => beforeSerialize;
            set => beforeSerialize = SanityCheckCallback(metaType.Model, value);
        }

        // Token: 0x170002BD RID: 701
        // (get) Token: 0x060032F9 RID: 13049 RVA: 0x001285C7 File Offset: 0x001269C7
        // (set) Token: 0x060032FA RID: 13050 RVA: 0x001285CF File Offset: 0x001269CF
        public MethodInfo BeforeDeserialize
        {
            get => beforeDeserialize;
            set => beforeDeserialize = SanityCheckCallback(metaType.Model, value);
        }

        // Token: 0x170002BE RID: 702
        // (get) Token: 0x060032FB RID: 13051 RVA: 0x001285E9 File Offset: 0x001269E9
        // (set) Token: 0x060032FC RID: 13052 RVA: 0x001285F1 File Offset: 0x001269F1
        public MethodInfo AfterSerialize
        {
            get => afterSerialize;
            set => afterSerialize = SanityCheckCallback(metaType.Model, value);
        }

        // Token: 0x170002BF RID: 703
        // (get) Token: 0x060032FD RID: 13053 RVA: 0x0012860B File Offset: 0x00126A0B
        // (set) Token: 0x060032FE RID: 13054 RVA: 0x00128613 File Offset: 0x00126A13
        public MethodInfo AfterDeserialize
        {
            get => afterDeserialize;
            set => afterDeserialize = SanityCheckCallback(metaType.Model, value);
        }

        // Token: 0x170002C0 RID: 704
        // (get) Token: 0x060032FF RID: 13055 RVA: 0x0012862D File Offset: 0x00126A2D
        public bool NonTrivial => beforeSerialize != null || beforeDeserialize != null || afterSerialize != null || afterDeserialize != null;

        // Token: 0x040023EC RID: 9196
        private readonly MetaType metaType;

		// Token: 0x040023ED RID: 9197
		private MethodInfo beforeSerialize;

		// Token: 0x040023EE RID: 9198
		private MethodInfo afterSerialize;

		// Token: 0x040023EF RID: 9199
		private MethodInfo beforeDeserialize;

		// Token: 0x040023F0 RID: 9200
		private MethodInfo afterDeserialize;
	}
}
