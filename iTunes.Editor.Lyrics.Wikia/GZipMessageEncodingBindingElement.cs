// -----------------------------------------------------------------------
// <copyright file="GZipMessageEncodingBindingElement.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Lyrics.Wikia
{
    using System;
    using System.ServiceModel.Channels;
    using System.Xml;

    /// <summary>
    /// This is the binding element that, when plugged into a custom binding, will enable the GZip encoder.
    /// </summary>
    internal sealed class GZipMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GZipMessageEncodingBindingElement"/> class with the specified <see cref="MessageEncodingBindingElement"/>.
        /// </summary>
        /// <remarks>By default, use the default text encoder as the inner encoder</remarks>
        public GZipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipMessageEncodingBindingElement"/> class with the specified <see cref="MessageEncodingBindingElement"/>.
        /// </summary>
        /// <param name="messageEncoderBindingElement">The message encoding binding element.</param>
        public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement) => this.InnerMessageEncodingBindingElement = messageEncoderBindingElement;

        /// <summary>
        /// Gets or sets the inner message encoding binding element.
        /// </summary>
        public MessageEncodingBindingElement InnerMessageEncodingBindingElement { get; set; }

        /// <inheritdoc />
        public override MessageVersion MessageVersion
        {
            get => this.InnerMessageEncodingBindingElement.MessageVersion;
            set => this.InnerMessageEncodingBindingElement.MessageVersion = value;
        }

        /// <inheritdoc />
        /// <remarks>Main entry point into the encoder binding element. Called by WCF to get the factory that will create the message encoder</remarks>
        public override MessageEncoderFactory CreateMessageEncoderFactory() => new GZipMessageEncoderFactory(this.InnerMessageEncodingBindingElement.CreateMessageEncoderFactory());

        /// <inheritdoc />
        public override BindingElement Clone() => new GZipMessageEncodingBindingElement(this.InnerMessageEncodingBindingElement);

        /// <inheritdoc />
        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return this.InnerMessageEncodingBindingElement.GetProperty<T>(context);
            }

            return base.GetProperty<T>(context);
        }

        /// <inheritdoc />
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }
    }
}
